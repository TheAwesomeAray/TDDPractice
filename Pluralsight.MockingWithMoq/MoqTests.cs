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
    }

    public interface ICustomerRepository
    {
        void Save(Customer customer);
    }

    public class CustomerService
    {
        private ICustomerRepository repo;
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

        public void Create(CustomerToCreateDto dto)
        {
            var customer = new Customer(dto);
            repo.Save(customer);
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
    }

    public interface IIdFactory
    {
        int Create();
    }

    public class MailingAddress
    {
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
    }

    public class Customer
    {
        public Customer(CustomerToCreateDto dto)
        {

        }
    }
}
