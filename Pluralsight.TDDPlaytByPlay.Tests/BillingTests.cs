using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Pluralsight.TDDPlaytByPlay.Tests
{
    public class BillingTests
    {
        [Fact]
        public void CustomerWhoDoesNotHaveSubscriptionDoesNotGetCharged()
        {
            var customer = new Customer();
            var processor = TestableBillingProcessor.Create(customer);

            processor.ProcessMonth(2011, 8);

            processor.Charger.Verify(c => c.ChargeCustomer(customer), Times.Never);
        }

        [Fact]
        public void CustomerWithSubscriptionThatIsExpiredGetsCharged()
        {
            var customer = new Customer() { Subscribed = true };
            var processor = TestableBillingProcessor.Create(customer);

            processor.ProcessMonth(2011, 8);

            processor.Charger.Verify(c => c.ChargeCustomer(customer), Times.Once);
        }

        [Fact]
        public void CustomerWithSubscriptionThatIsCurrentDoesNotGetCharged()
        {
            var customer = new Customer() { Subscribed = true, PaidThroughYear = 2012, PaidThroughMonth = 5 };
            var processor = TestableBillingProcessor.Create(customer);

            processor.ProcessMonth(2011, 8);

            processor.Charger.Verify(c => c.ChargeCustomer(customer), Times.Never);
        }

        //Monthly Billing
        //Grace Period for missed payments ("dunning" status)
        //Not all customers are necessarily subscribers
        //Idle customers should be auto unsubscribed
        //What about customers who sign up today?
    }

    public interface ICustomerRepository
    {
        IEnumerable<Customer> Customers { get; }
    }

    public interface ICreditCardCharger
    {
        void ChargeCustomer(Customer customer);
    }

    public class Customer
    {
        public bool Subscribed { get; set; }
        public int PaidThroughYear { get; set; }
        public int PaidThroughMonth { get; set; }
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
            if (customer.Subscribed && 
                (customer.PaidThroughYear <= year && customer.PaidThroughMonth < month))
            {
                charger.ChargeCustomer(customer);
            }
                
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
}
