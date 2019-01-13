using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace Pluralsight.MockingWithMoq
{
    public class MoqTests
    {
        [Fact]
        public void TheRepositorySaveShouldBeCalled()
        {
            var mockRepository = new Mock<ICustomerRepository>();
            mockRepository.Setup(x => x.Save(It.IsAny<Customer>()));
            var customerService = new CustomerService(mockRepository.Object);

            customerService.Create(new CustomerToCreateDto());

            mockRepository.VerifyAll();
        }

        [Fact]
        public void SaveShouldBeCalledOncePerCustomer()
        {
            var listOfCustomerDtos = new List<CustomerToCreateDto>()
            {
                new CustomerToCreateDto(),
                new CustomerToCreateDto(),
                new CustomerToCreateDto()
            };
            var mockRepository = new Mock<ICustomerRepository>();
            var customerService = new CustomerService(mockRepository.Object);

            customerService.Create(listOfCustomerDtos);

            mockRepository.Verify(x => x.Save(It.IsAny<Customer>()), 
                Times.Exactly(listOfCustomerDtos.Count));
        }
        
        //Controlling Return Values
        [Fact]
        public void AnExceptionShouldBeThrownIfAnAddressIsNotCreated()
        {
            var mockAddressBuilder = new Mock<ICustomerAddressBuilder>();
            var mockRepository = new Mock<ICustomerRepository>();
            var customerService = new CustomerService(mockAddressBuilder.Object, mockRepository.Object);

            mockAddressBuilder.Setup(x => x.From(It.IsAny<CustomerToCreateDto>())).Returns(null);

            Assert.Throws<InvalidOperationException>(() => customerService.Create());
        }

        //Parameters
        [Fact]
        public void AnExceptionShouldBeThrownIfMailingAddressIsNotParsed()
        {
            var mockFactory = new Mock<IMailingAddressFactory>();
            var mockRepository = new Mock<ICustomerRepository>();
            var customerService = new CustomerService(mockFactory.Object, mockRepository.Object);
            var mailingAddress = new MailingAddress();
            mockFactory.Setup(x => x.TryParse(It.IsAny<string>(), out mailingAddress));

            customerService.MailAddressTest();

            mockRepository.Verify(x => x.Save(It.IsAny<Customer>()));
        }

        [Fact]
        public void EachCustomerShouldBeAssignedAnId()
        {
            var listOfCustomerDtos = new List<CustomerToCreateDto>()
            {
                new CustomerToCreateDto(),
                new CustomerToCreateDto(),
                new CustomerToCreateDto()
            };
            var mockRepository = new Mock<ICustomerRepository>();
            var mockIdFactory = new Mock<IIdFactory>();
            var customerService = new CustomerService(mockRepository.Object, mockIdFactory.Object);
            var i = 1;
            mockIdFactory.Setup(x => x.Create())
                         .Returns(i)
                         .Callback(() => i++);

            customerService.IdFactoryTest(listOfCustomerDtos);

            mockIdFactory.Verify(x => x.Create(), Times.AtLeastOnce());
        }

        [Fact]
        public void FullNameShouldBeCreatedFromFirstAndLastName()
        {
            var dto = new CustomerToCreateDto() { FirstName = "Bob", LastName = "Builder" };
            var mockRepository = new Mock<ICustomerRepository>();
            var mockFullNameBuilder = new Mock<ICustomerFullNameBuilder>();
            var customerService = new CustomerService(mockRepository.Object, mockFullNameBuilder.Object);

            customerService.CustomerFullNameBuilderTest(dto);

            //Ensure correct values are being passed into a method
            mockFullNameBuilder.Verify(x => x.From(
                It.Is<string>(fn => fn.Equals(dto.FirstName, StringComparison.InvariantCultureIgnoreCase)),
                It.Is<string>(fn => fn.Equals(dto.LastName, StringComparison.InvariantCultureIgnoreCase))));
        }

        [Fact]
        public void TheLocalTimeZoneShouldBeSet()
        {
            var mockCustomerRepository = new Mock<ICustomerRepository>();
            var customerService = new CustomerService(mockCustomerRepository.Object);

            customerService.Create(new CustomerToCreateDto());

            //Test is a property setter was called
            mockCustomerRepository.VerifySet(x => x.LocalTimeZone = It.IsAny<string>());
        }

        [Fact]
        public void TheLocalTimeZoneShouldBeUsed()
        {
            var mockCustomerRepository = new Mock<ICustomerRepository>();
            mockCustomerRepository.Setup(x => x.LocalTimeZone).Returns("Test");
            var customerService = new CustomerService(mockCustomerRepository.Object);

            customerService.Create(new CustomerToCreateDto());

            //Test is a property setter was called
            mockCustomerRepository.VerifyGet(x => x.LocalTimeZone);
        }

        [Fact]
        public void MockingWithLawOfDemeterViolations()
        {
            var mockCustomerRepository = new Mock<ICustomerRepository>();
            mockCustomerRepository.Setup(x => x.Customer.MailingAddress.Address).Returns("Test");
            var customerService = new CustomerService(mockCustomerRepository.Object);

            customerService.Create(new CustomerToCreateDto());

            //Test is a property setter was called
            mockCustomerRepository.VerifyGet(x => x.LocalTimeZone);
        }

        [Fact]
        public void StubbingProperties()
        {
            //I want to set values, not just have them return a fixed value
            var mockCustomerRepository = new Mock<ICustomerRepository>();
            //There is an overload that lets us set the initial value as well
            mockCustomerRepository.SetupProperty(x => x.LocalTimeZone, "Test1");
            //Can also set up all properties
            //mockCustomerRepository.SetupAllProperties();
            var customerService = new CustomerService(mockCustomerRepository.Object);

            customerService.Create(new CustomerToCreateDto());

            //Test is a property setter was called
            mockCustomerRepository.VerifyGet(x => x.LocalTimeZone);
        }

        [Fact]
        public void MockingEvents()
        {
            var mockCustomerRepository = new Mock<ICustomerRepository>();
            var mockMailingRepository = new Mock<IMailingRepository>();
            var customerService = new CustomerService(mockCustomerRepository.Object, mockMailingRepository.Object);

            mockCustomerRepository.Raise(x => x.NotifySalesTeam += null, new NotifySalesTeamEventArgs(""));

            mockMailingRepository.Verify(x => x.NewCustomerMessage(It.IsAny<string>()));
        }
    }

    public interface ICustomerRepository
    {
        string LocalTimeZone { get; set; }

        Customer Customer { get; set; }
        event EventHandler<NotifySalesTeamEventArgs> NotifySalesTeam;
        IMailingRepository MailingRepository { get; set; }

        void Save(Customer customer);
        void FetchAll();
    }

    public class CustomerService
    {
        private ICustomerRepository repo;
        private IMailingRepository mailingRepository;
        private ICustomerFullNameBuilder customerFullNameBuilder;
        private IIdFactory idFactory;
        private ICustomerAddressBuilder addressBuilder;
        private IMailingAddressFactory factory;
        public CustomerService(ICustomerRepository repo)
        {
            this.repo = repo;
        }

        public CustomerService(ICustomerAddressBuilder addressBuilder, ICustomerRepository repo)
        {
            this.repo = repo;
            this.addressBuilder = addressBuilder;
        }

        public CustomerService(IMailingAddressFactory factory, ICustomerRepository repo)
        {
            this.repo = repo;
            this.factory = factory;
        }

        public CustomerService(ICustomerRepository repo, IIdFactory idFactory)
        {
            this.repo = repo;
            this.idFactory = idFactory;
        }

        public CustomerService(ICustomerRepository repo, ICustomerFullNameBuilder customerFullNameBuilder)
        {
            this.repo = repo;
            this.customerFullNameBuilder = customerFullNameBuilder;
        }

        public CustomerService(ICustomerRepository repo, IMailingRepository mailingRepository)
        {
            this.repo = repo;
            this.mailingRepository = mailingRepository;

            repo.NotifySalesTeam += NotifySalesTeam;
        }
        
        public void NotifySalesTeam(object sender, NotifySalesTeamEventArgs e)
        {
            mailingRepository.NewCustomerMessage(e.Name);
        }

        public void Create(CustomerToCreateDto dto)
        {
            var customer = new Customer(dto);

            //Despite setting the value to Test2, the mock still will return the value specified in the Setup
            //Unless the SetupProperty is used. In which case this will be properly get and set
            repo.LocalTimeZone = "Test2";
            var test = repo.LocalTimeZone;

            repo.Save(customer);
            repo.FetchAll();
        }

        public void Create()
        {
            var result = addressBuilder.From(new CustomerToCreateDto());

            if (result == null)
            {
                throw new InvalidOperationException();
            }
        }

        public void MailAddressTest()
        {
            MailingAddress address; 
            factory.TryParse("123 Test St", out address);

            if (address == null)
                throw new InvalidOperationException();

            repo.Save(new Customer(new CustomerToCreateDto()));
        }

        public void Create(IEnumerable<CustomerToCreateDto> customersToCreate)
        {
            foreach (var customerToCreate in customersToCreate)
            {
                repo.Save(new Customer(customerToCreate));
            }
        }

        internal void IdFactoryTest(List<CustomerToCreateDto> customersToCreate)
        {
            foreach (var customerToCreate in customersToCreate)
            {
                customerToCreate.Id = idFactory.Create();
                repo.Save(new Customer(customerToCreate));
            }
        }

        internal void CustomerFullNameBuilderTest(CustomerToCreateDto dto)
        {
            customerFullNameBuilder.From(dto.FirstName, dto.LastName);
        }

        internal void RecursiveMockingTest()
        {
            repo.MailingRepository.From(new CustomerToCreateDto());
        }
    }

    public class NotifySalesTeamEventArgs : EventArgs
    {
        private string name;

        public NotifySalesTeamEventArgs(string name)
        {
            this.name = name;
        }

        public object Name { get; internal set; }
    }

    public interface IMailingRepository
    {
        void From(CustomerToCreateDto customerToCreateDto);
        void NewCustomerMessage(object name);
    }

    public interface ICustomerFullNameBuilder
    {
        void From(string firstName, string lastName);
    }

    public interface IIdFactory
    {
        int Create();
    }

    public class MailingAddress
    {
        public virtual string Address { get; set; }
    }

    public interface IMailingAddressFactory
    {
        void TryParse(string v, out MailingAddress address);
    }

    public interface ICustomerAddressBuilder
    {
        object From(CustomerToCreateDto customerToCreateDto);
    }

    public class CustomerToCreateDto
    {
        public int Id { get; set; }
        public string FirstName { get; internal set; }
        public string LastName { get; internal set; }
    }

    public class Customer
    {
        public virtual MailingAddress MailingAddress { get; set; }
        public string FirstName { get; internal set; }
        public string LastName { get; internal set; }

        public Customer()
        {

        }

        public Customer(CustomerToCreateDto dto)
        {

        }
    }
}
