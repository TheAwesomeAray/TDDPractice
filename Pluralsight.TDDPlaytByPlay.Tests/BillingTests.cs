using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;


public class BillingTests
{
    public class NoSubscription
    {
        [Fact]
        public void CustomerWhoDoesNotHaveSubscriptionDoesNotGetCharged()
        {
            var customer = new Customer();
            var processor = TestableBillingProcessor.Create(customer);

            processor.ProcessMonth(2011, 8);

            processor.Charger.Verify(c => c.ChargeCustomer(customer), Times.Never);
        }
    }

    public class Monthly
    {
        [Fact]
        public void CustomerWithSubscriptionThatIsExpiredGetsCharged()
        {
            var subscription = new MonthlySubscription() { PaidThroughYear = 2011, PaidThroughMonth = 8 };
            var customer = new Customer() { Subscription = subscription };
            var processor = TestableBillingProcessor.Create(customer);

            processor.ProcessMonth(2011, 9);

            processor.Charger.Verify(c => c.ChargeCustomer(customer), Times.Once);
        }

        [Fact]
        public void CustomerWithSubscriptionThatIsCurrentDoesNotGetCharged()
        {
            var subscription = new MonthlySubscription() { PaidThroughYear = 2012, PaidThroughMonth = 8 };
            var customer = new Customer() { Subscription = subscription };
            var processor = TestableBillingProcessor.Create(customer);

            processor.ProcessMonth(2011, 8);

            processor.Charger.Verify(c => c.ChargeCustomer(customer), Times.Never);
        }

        [Fact]
        public void CustomerWhoIsSubscribedButFailsToPayMaximumTimesIsNoLongerSubscribed()
        {
            var subscription = new MonthlySubscription() { PaidThroughYear = 2011, PaidThroughMonth = 8 };
            var customer = new Customer() { Subscription = subscription };
            var processor = TestableBillingProcessor.Create(customer);
            processor.Charger.Setup(c => c.ChargeCustomer(It.IsAny<Customer>()))
                             .Returns(false);

            for (int i = 0; i < MonthlySubscription.MAX_FAILURES; i++)
                processor.ProcessMonth(2012, 8);

            Assert.False(customer.Subscription.IsCurrent);
        }

        [Fact]
        public void CustomerWhoIsSubscribedButFailsToPayOnceIsStillSubscribed()
        {
            var subscription = new MonthlySubscription() { PaidThroughYear = 2011, PaidThroughMonth = 8 };
            var customer = new Customer() { Subscription = subscription };
            var processor = TestableBillingProcessor.Create(customer);
            processor.Charger.Setup(c => c.ChargeCustomer(It.IsAny<Customer>()))
                             .Returns(false);

            processor.ProcessMonth(2011, 8);

            Assert.True(customer.Subscription.IsCurrent);
        }
    }

    public class Annual
    {
    }

    //Grace Period for missed payments ("dunning" status)
    //Idle customers should be auto unsubscribed
    //What about customers who sign up today?
}

public interface ICustomerRepository
{
    IEnumerable<Customer> Customers { get; }
}

public interface ICreditCardCharger
{
    bool ChargeCustomer(Customer customer);
}

public abstract class Subscription
{
    public abstract bool IsCurrent { get; }
    public abstract bool IsRecurring { get; }
    protected int chargeFailures { get; set; }
    public abstract bool NeedsBilling(int year, int month);
    public virtual void RecordChargeResult(bool charged)
    {
        if (!charged)
            chargeFailures++;
    }
}

public class AnnualSubscription : Subscription
{
    public override bool IsRecurring => false;

    public override bool IsCurrent => throw new NotImplementedException();

    public override bool NeedsBilling(int year, int month)
    {
        throw new NotImplementedException();
    }
}

public class MonthlySubscription : Subscription
{
    public const int MAX_FAILURES = 3;
    public override bool IsCurrent => chargeFailures < MAX_FAILURES;
    public override bool IsRecurring => true;
    public int PaymentFailures { get; set; }
    public int PaidThroughYear { get; set; }
    public int PaidThroughMonth { get; set; }

    public override bool NeedsBilling(int year, int month)
    {
        return !IsCurrent || (PaidThroughYear <= year && PaidThroughMonth <= month);
    }
}

public class Customer
{
    public Subscription Subscription { get; set; }
}

public class BillingProcessor
{
    private ICustomerRepository repo;
    private ICreditCardCharger charger;

    public BillingProcessor(ICustomerRepository repo, ICreditCardCharger charger)
    {
        this.repo = repo;
        this.charger = charger;
    }

    internal void ProcessMonth(int year, int month)
    {
        var customer = repo.Customers.Single();
        if (NeedsBilling(customer, year, month))
        {
            bool charged = charger.ChargeCustomer(customer);
            customer.Subscription.RecordChargeResult(charged);
        }
    }

    private bool NeedsBilling(Customer customer, int year, int month)
    {
        return customer.Subscription != null
            && customer.Subscription.NeedsBilling(year, month);
    }
}

public class TestableBillingProcessor : BillingProcessor
{
    public Mock<ICreditCardCharger> Charger;
    public Mock<ICustomerRepository> Repository;

    TestableBillingProcessor(Mock<ICreditCardCharger> charger,
                                    Mock<ICustomerRepository> repository)
        : base(repository.Object, charger.Object)
    {
        Charger = charger;
        Repository = repository;
    }

    public static TestableBillingProcessor Create(params Customer[] customers)
    {
        Mock<ICustomerRepository> repository = new Mock<ICustomerRepository>();
        repository.Setup(r => r.Customers)
                  .Returns(customers);

        return new TestableBillingProcessor(
            new Mock<ICreditCardCharger>(),
            repository
        );
    }
}

