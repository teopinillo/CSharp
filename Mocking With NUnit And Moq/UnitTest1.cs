using System;
using CostumerService;
using NUnit;
using NUnit.Framework;
using Moq;
using Moq.Protected;

namespace CostumerServiceTest
{
    [TestFixture]
    public class UnitTest1
    {
        private Mock<ICustomerRepository> mockCustomerRepository;
        private Mock<ICustomerAddressBuilder> mockAddressBuilder;
        private Mock<ICustomerStatusFactory> mockCustomerStatusFactory;
        private Mock<ICustomerFullNameBuilder> mockFullNameBuilder;
        private Mock<ISecurityLogin> mockSecurityLogin;
        private CustomerToCreateDto customerToCreateDto;
        private CustomerService customerService;
        private Mock<IApplicationSettings> mockApplicationSettings;
        private Mock<IMailingRepository> mockMailingRepository;


        [SetUp]
        public void Init()
        {
            //Arrange
           customerToCreateDto = new CustomerToCreateDto
            {
                Firstname = "Bob",
                Lastname = "Builder",
                DesireStatus = CustomerStatus.Platinum
            };

            mockCustomerRepository = new Mock<ICustomerRepository>();
            mockAddressBuilder = new Mock<ICustomerAddressBuilder>();
            mockCustomerStatusFactory = new Mock<ICustomerStatusFactory>();
            mockFullNameBuilder = new Mock<ICustomerFullNameBuilder>();
            mockSecurityLogin = new Mock<ISecurityLogin>();
            mockApplicationSettings = new Mock<IApplicationSettings>();
            mockMailingRepository = new Mock<IMailingRepository>();

            mockAddressBuilder.Setup(x => x.From(It.IsAny<CustomerToCreateDto>()))
               .Returns(new Address());

            mockApplicationSettings.Setup(x => x.WorkstationId).Returns(301);
            mockMailingRepository.Setup(x => x.NewCustomerMessage(It.IsAny<string>()));

            customerService = new CustomerService(
                mockCustomerRepository.Object,
                mockAddressBuilder.Object,
                mockSecurityLogin.Object)
            {
                _fullNameBuilder = mockFullNameBuilder.Object,
                _customerStatusFactory = mockCustomerStatusFactory.Object
                          
            };
            customerService._applicationSettings = mockApplicationSettings.Object;
            customerService._mailingRepository = mockMailingRepository.Object;
        }

        //this show how setting the return value will change the execution flow
        [Test]
        public void Create_An_Exception_Should_Be_Thrown_If_The_Address_Is_Not_Created()
        {
            //Arrange => Init()
            mockAddressBuilder.Setup(x => x.From(It.IsAny<CustomerToCreateDto>()))
              .Throws<InvalidCustomerMailingAddressException>();

            customerService = new CustomerService(
                mockCustomerRepository.Object,
                mockAddressBuilder.Object,
                mockSecurityLogin.Object)
            {
                _fullNameBuilder = mockFullNameBuilder.Object,
                _customerStatusFactory = mockCustomerStatusFactory.Object
            };
            
            //Act            
            Assert.That(
                () => customerService.Create(customerToCreateDto),
                Throws.TypeOf<CustomerCreationException>());
        }

        [Test]
        public void GetTmpLoginCode_When_Called_With_Customer_Should_Return_An_Integer()
        {
            //Arrange
            var customer = new Customer("Bob", "Builder");
            mockSecurityLogin.Setup(x => x.GetTempLoginCode()).Returns(101202);
            
            //Act
            bool status = customerService.GetTmpLoginCode(customer, out int tmpCode);

            //Assert
            Assert.IsTrue(status);
            Assert.AreEqual(tmpCode, 101202);
        }

        //Multiple Return Value Demo
        [Test]
        public void GetTmpLoginCode_WhenCalled_ReturnDifferentCode()
        {
            //Arrange
            var customer = new Customer("Bob", "Builder");

            Random random = new Random();
            int t = random.Next(0, 999999);           
            mockSecurityLogin.Setup(x => x.GetTempLoginCode())
                .Returns(() => t)
                .Callback(() => t = random.Next(0, 999999));
            CustomerService customerService = new CustomerService(
                mockCustomerRepository.Object, 
                mockAddressBuilder.Object, 
                mockSecurityLogin.Object);

            //Act
            customerService.GetTmpLoginCode(customer, out int FirstCode);
            customerService.GetTmpLoginCode(customer, out int SecondCode);
            Assert.AreNotEqual(FirstCode, SecondCode);
        }

        [Test]
        //Demo Argument Tracking
        public void A_Full_Name_Should_Be_Created_From_First_And_Last_name()
        {
            //Arrange          

            mockFullNameBuilder.Setup(
                x => x.From(It.IsAny<string>(), It.IsAny<string>()));
            customerService._fullNameBuilder = mockFullNameBuilder.Object;           

            //Act
            customerService.Create(customerToCreateDto);

            //Assert
            mockFullNameBuilder.Verify(x => x.From(
               It.Is<string>(
                   fn => fn.Equals(customerToCreateDto.Firstname,
                   StringComparison.InvariantCultureIgnoreCase)),
                   It.Is<string>(fn => fn.Equals(customerToCreateDto.Lastname,
                   StringComparison.InvariantCultureIgnoreCase))));
        }


        [Test]
        //Demo Argument Tracking
        public void If_Customer_Is_Platinum_Should_Save_Special()
        {
            
            //This was my implementation, it also work fine
            //mockCustomerStatusFactory
            //    .Setup(x => x.CreateFrom(It.IsAny<CustomerToCreateDto>()))
            //    .Returns(CustomerStatus.Platinum);

            mockCustomerStatusFactory.Setup(
                x => x.CreateFrom(
                    It.Is<CustomerToCreateDto>(
                        y => y.DesireStatus == CustomerStatus.Platinum)))
                        .Returns(CustomerStatus.Platinum);

            customerService._customerStatusFactory = mockCustomerStatusFactory.Object;           
            
            //Act
            customerService.Create(customerToCreateDto);

            //Assert
            mockCustomerRepository.Verify(x=>x.SaveSpecial(It.IsAny<Customer>()));
        }

        [Test]     
        //Demo Catch Exception
        public void Create_Catch_InvalidCustomerMailingAdddressException()
        {
            //Arrange
            mockAddressBuilder.Setup(x => x.From(It.IsAny<CustomerToCreateDto>()))
               .Throws<InvalidCustomerMailingAddressException>();
            
            var customerService = new CustomerService(
                mockCustomerRepository.Object,
                mockAddressBuilder.Object,
                null)
            {
                _fullNameBuilder = mockFullNameBuilder.Object,
                _customerStatusFactory = mockCustomerStatusFactory.Object
            };            
            //Assert
            Assert.That( () => customerService.Create(customerToCreateDto),
                Throws.TypeOf<CustomerCreationException>());
        }

        [Test]
        //Demo Verify a Set Properties
        public void Create_When_Called_Set_TimeZone_To_System_Time_Zone()
        {
         //Arrange => Init();
         
         //Act
         customerService.Create(customerToCreateDto);      
         //Assert
         mockCustomerRepository.VerifySet(x => x.LocalTimeZone = It.IsAny<string>(), Times.Once);
         
        }

        //Demo Mocking Property Getter
        [Test]
        public void Create_When_Called_Should_Access_WorkstationID()
        {
            customerService.Create(customerToCreateDto);
            mockApplicationSettings.Verify(x => x.WorkstationId);
        }

        //Demo raising event
        [Test]
        public void When_Creating_A_New_Customer_Rise_Event()
        {
            //Act
            mockCustomerRepository.Raise (
                x=> x.NotifySalesTeam += null,
                new NotifySalesTeamEventArgs ("jim"));
            //Assert
            mockMailingRepository.Verify(x => x.NewCustomerMessage(It.IsAny<string>()));
        }

        //Demo raising event using delegate
        [Test]
        public void When_Creating_A_New_Platinum_Customer_Rise_Event_Notify_Manager()
        {
            //Act
            mockCustomerRepository.Raise(
                x => x.NotifyingSalesTeamManager += null,
                "theo",false);
            //Assert
            mockMailingRepository.Verify(x => x.NewCustomerMessage(It.IsAny<string>(),It.IsAny<bool>()));
        }

        [Test]
        //Demo Testing Base Clase Implementations
        public void UspsAddress_IsValid_Should_Be_Called_once()
        {
            //Arrange agains the child class
            Mock<AddressFormatter> mockAddressFormatter = new Mock<AddressFormatter>();

            //Act
            mockAddressFormatter.Object.Parse("any address");

            //Assert
            mockAddressFormatter.Verify(
                x => x.IsValid(It.IsAny<string>()), Times.Once);

        }

        [Test]
        //Demo, testing Protected members
        public void Testing_Protected_Members()
        {
            //Arrange, our system under test
            var mockAddresFormatter = new Mock<AddressFormatter>();
            //Declare the expectation
            mockAddresFormatter.Protected()
                .Setup<string>("ProtectedFunction",  //The protected member we want to verify
                ItExpr.IsAny<string>())
                .Returns("test")
                .Verifiable();          //mark as Verifiable to be able to verify

            //Act
            mockAddresFormatter.Object.CallingFunction();

            //Assert
            mockAddresFormatter.Verify();
        }
    }
}
