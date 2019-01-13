using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Moq.Protected;

namespace Pluralsight.MockingWithMoq
{
    public class AdvanceMocking
    {
        //Strict and Loose Mocking
        [Fact]
        public void StrictMockExample()
        {
            var mockrepo = new Mock<ICustomerRepository>(MockBehavior.Strict);
            mockrepo.Setup(x => x.Save(It.IsAny<Customer>()));
            var customerService = new CustomerService(mockrepo.Object);

            customerService.Create(new CustomerToCreateDto());

            //Will fail, because items used are not setup
            mockrepo.Verify();
        }

        [Fact]
        public void BaseClassImplementationsAKAPartialMocks()
        {
            //Often used with Web/Html controls in System.Web
            var mockNameFormatter = new Mock<CustomerNameFormatter>();

            mockNameFormatter.Object.From(new Customer(new CustomerToCreateDto()));

            mockNameFormatter.Verify(x => x.ParseBadWordsFrom(It.IsAny<string>()), Times.Exactly(2));
        }

        [Fact]
        public void RecursiveMocking()
        {
            var mockCustomerRepo = new Mock<ICustomerRepository>() { DefaultValue = DefaultValue.Mock };
            //Default Value of Mock sets the Mock to try and Mock all its members

            var customerService = new CustomerService(mockCustomerRepo.Object);
            var mailingAddressRepo = mockCustomerRepo.Object.MailingRepository;
            var mock = Mock.Get(mailingAddressRepo);

            customerService.RecursiveMockingTest();

            mock.Verify(x => x.From(It.IsAny<CustomerToCreateDto>()));
        }

        [Fact]
        public void MockRepositoryCreation()
        {
            //I can create a mockRepository and use it to create future mocks, defining the rules they should follow. I can
            //define their behavior in one place
            var mockFactory = new MockRepository(MockBehavior.Loose)
            { DefaultValue = DefaultValue.Mock };
            var mockCustomerRepository = mockFactory.Create<ICustomerRepository>();

            //I can also verify all the mocks created by the factory, avoiding a chain of verify calls
            mockFactory.Verify();
        }

        [Fact]
        public void ProtectedMembersExample()
        {
            var mockNameFormatter = new Mock<CustomerNameFormatterProtectedExample>();

            //Caveats - No intellisense
            mockNameFormatter.Protected()
                 .Setup<string>("ParseBadWordsFrom", ItExpr.IsAny<string>())
                 .Returns("asdf")
                 .Verifiable();
            mockNameFormatter.Object.From(new Customer(new CustomerToCreateDto()));
            
            mockNameFormatter.Verify();
        }

        //Base class implementations
        //Recursive mocking
        //Centralized Mock Creation
        //Protected members


    }

    public class CustomerNameFormatter : BaseFormatter
    {
        public string From(Customer customer)
        {
            var firstName = ParseBadWordsFrom(customer.FirstName);
            var lastName = ParseBadWordsFrom(customer.LastName);

            return string.Format("{0}, {1}", lastName, firstName);
        }
    }

    public class CustomerNameFormatterProtectedExample : BaseFormatterProtectedExample
    {
        public string From(Customer customer)
        {
            var firstName = ParseBadWordsFrom(customer.FirstName);
            var lastName = ParseBadWordsFrom(customer.LastName);

            return string.Format("{0}, {1}", lastName, firstName);
        }
    }

    public class BaseFormatterProtectedExample
    {
        protected virtual string ParseBadWordsFrom(string source)
        {
            return "";
        }
    }

    public class BaseFormatter
    {
        public virtual string ParseBadWordsFrom(string source)
        {
            return "";
        }
    }
}
