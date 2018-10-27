//Don't Test private methods
//If need refactoring, extract an interface from the class that touch external resources
//like db, file system, etc.
//and mock this interface.
// make the class loosely couple using injections
//is possible to mock classes, but the method to mock must be virtual
//is possible mock Virtual Protected Members with: using Moq.Protected;
//Stubbing: Be able to set up objects and properties and have their values
// both configured for return as well as be able to change them going forward.

// tools > NUGet Package Manager > Package Manager Console
// install-package moq
// install-package NUnit -Version 3.11.0
// install-package NUnit3TestAdapter –version 3.10.0

//[TestFixture]
//[TestFixtureSetup]
//[TestFextureTearDown]
//[Test]  : method
//[TestCase (param1,...,paramN)] : method (param1...pramN)
//tell the system we are excepting an exception to occur.
//[ExpectedException (typeof(ArgumentException))]   ---> Not longer supported in NUnit 3.+

//Assert.AreEqual (value1, value2);
//Assert.AreNotEqual (value1, value2);
//Assert.AreNotSame
//Assert.AreSame
//Assert.ByVal
//Assert.Catch
//Assert.Catch<>
//Assert.CatchAsync
//Assert.CatchAsync<>
//Assert.False
//Assert.True
//Assert.Contains
//Assert.Fail
//Assert.DoesNotThrow
//Assert.DoesNotThrowAsync
//Assert.Equals
//Assert.Fail
//Assert.False
//Assert.Greater
//Assert.GreaterOrEqual
//Assert.Ignore
//Assert.Inclusive
//Assert.IsAssignableFrom
//Assert.IsAssignableFrom<>
//Assert.IsEmpty
//Assert.IsFalse
//Assert.IsInstanceOf
//Assert.IsInstanceOf<>
//Assert.IsNaN
//Assert.IsNotAssignableFrom
//Assert.IsNotAssignableFrom<>
//Assert.IsNotEmpty
//Assert.IsNotInstanceOf
//Assert.IsNotInstanceOf<>
//Assert.IsNotNull
//Assert.IsNull
//Assert.IsTrue
//Assert.IsNull
//Assert.IsTrue
//Assert.Less
//Assert.LessOrEqual
//Assert.Multiple
//Assert.Negative
//Assert.NonNull
//Assert.NotZero
//Assert.Null
//Assert.Pass
//Assert.Positive
//Assert.ReferenceEquals
//Assert.That
//Assert.That<>
//Assert.Throws
//Assert.Throws<>
//Assert.ThrowsAsyn
//Assert.ThrowsAsyn<>
//Assert.True
//Assert.Warn
//Assert.Zero

//-----------------------------------Times
//Times.AtLeast
//Times.AtLeastOnce
//Times.Once
//TImes.AtMost
//Time.AtMostOnce
//Time.Between 
//Time.Equals
//Time.Never
//Time.Exactly		: mock.Verify ( x=>x.MyMethod (), Times.Exactly(2));
//Time.ReferenceEquals

//-----------------------------------It.
//Equals
//Is<>
//IsAny<>
//IsIn<>
//IsInRange<>
//IsNotIn<>
//IsNotNull<>		: mock.Verify ( x=>x.MyMethod (It.IsNotNull<string>()), Times.Once, My_Failure_Message_Optional);
//IsRegex
//Ref<>
//ReferenceEquals

//----------------------------Throwing Exceptions, Mocking Exceptions
//
//  (1) Mock object throws when invoked, (2) Verify SUT exception handling
//
//  my_mock.Setup ( x => x.My_Method (It.IsAny<string>())).Throws<Exception>();
//  my_mock.Setup ( x => x.My_Method (It.IsAny<string>())).Throws (new Exception ("Custom message"));
//Assert
//            Assert.That( () => customerService.Create(customerToCreateDto),
//                               Throws.TypeOf<CustomerCreationException>());

//----------------------------Raise Events
// mockValidator.Raise ( lambda_expression_event_to_raise, event_arguments);

//----------------------------Setup multiple returns value 
//           mockValidator.SetupSequence(x => x.IsValid(It.IsAny<string>()))
//                .Returns(false)
//                .Returns(true);
		
//--------------LINQ to mock, improve mock setup readable           
//IMyInterfaz mockValidator
//                = Mock.Of<IMyInterfaz>
//                (
//                 validator => 
//                 validator.a_property == "OK" &&
//                 validator.a_method ( It.IsAny<string>()) == true
//                );		

//Optional Argument
// ...(int? my_var= null)
//default value in parameters
// int days = 1
//naming parameters days : 2



//mockValidator.Setup(...).Returns(true);
//my_mock.Setup (x => x.IsValid(It.IsRegex("[a-z]",
//		System.Text.RegularExpressions.RegexOptions.None)))
//		.Returns(true);
//mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true);
//mockValidator.Setup(x => x.IsValid(It.IsAny<string>( number => number.StartsWith('x'))).Returns(true);
//mockValidator.Setup(x => x.IsValid(It.IsInRange("x","y","z",Range.Inclusive))).Returns(true);

//-----------------------------------------------------------------------------------------------------Mock Behavior
// new Mock<Interface>( MockBehavior.Type );
//MockBehavior.Strict : Throw an exception if a mocked method is called but has not been setup.
//MockBehavior.Loose: Never throw exceptions, even if a mocked method is called has not been setup.
//                    Returns default values for value types, null for reference types, 
//					  empty array/enumerable.
//					- Less Line of code, Default Values
//MockBehavior.Default: Default behavior if none specified (MockBehavior.Loose)
//					-Has to setup each called method.
//*****USE STRICK MOCKS ONLY WHEN ABSOLUTELY NECESSARY, PREFER LOSE MOCKS AT ALL OTHER TIMES.

//https://stackoverflow.com/questions/24036384/create-mocks-with-auto-filled-properties-with-moq
//You can use chained mock creation (aka specification queries) to setup mock more quickly
//
//IRegistrationView view = Mock.Of<IRegistrationView>(ctx =>
//    ctx.Address == "Calle test" &&
//    ctx.EmailAddress == "test@test.com" &&
//    ctx.Password == "testpass" &&
//    ctx.FirstName == "Name" &&
//    ctx.LastName == "Surname");

// my_mock.DefaultValue = DefaultValue.Mock;
// Default value for value types, null for reference or empty for arrays or enumerable.
// String is a Sealed class, so, it will return null.
// int = 0, bool = false, 

//---------------------stubbing properties----------------------------------------------------------
//By default mock properties don't remember changes made to them during the execution of the test.
//In other words they don't track changes to their values. To tell Moq to keep track of the
//changes we use:
// my_mock.SetupProperty ( x=> x.MyProperty, valueA);
// my_mock.Object.MyProperty = another_value;
// my_mock.SetupAllProperties ();

//--------------------------------------------------------Verifying a method is called
//my_mock.Verify ( x => x.methodToVerify (argument));
//We can use argument matching: It.IsAny<string>(), for example.
//we can use a second argument Times, 
//my_mock.Verify ( x => x.methodToVerify (argument),Times.AtLeastOnce);
//my_mock.Verify ( x => x.methodToVerify (argument),Times.Exactly(2));

//--------------------------------------------------------Verifying a property Getter and Setter
// my_mock.VerifyGet (x=>x.ServiceInformation.License.LicenseKey); //Mocking Property Hierarchies.
// my_mock.VerifySet (x => x.ValidationMode = ValidationMode.Detailed);

//Mocking out argument
//Act
// bool status = customerService.GetTmpLoginCode(customer, out int tmpCode);
//Assert
// Assert.IsTrue(status);
// Assert.AreEqual(tmpCode, 101202);

//-------------------------------------------------------------------Testing Protected Members
 using Moq.Protected;
    public class AddressFormatter 
    {
        public void CallingFunction()
        {
            string r = ProtectedFunction("a");
        }
        protected virtual string ProtectedFunction (string s)
        {
            return "5555";
        }       
    }
	
//Testing Class
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
                .Returns("any value")
                .Verifiable();          //mark as Verifiable to be able to verify
            //Act
            mockAddresFormatter.Object.CallingFunction();
            //Assert
            mockAddresFormatter.Verify();
        }

//------------------------------------------------------------------------------------------------		


public void Add_ResultIsAPrimeNumber_ResultAreSaved()
 {
   Mock<IStore> mockstore = new Mock<Store>();
   StringCalculator calc = new StringCalculator (mockstore.Object);
   var result = calc.Add("3,4");
   //Verify that the method is called
   mockstore.Verify ( m => m.Save(It.IsAny<int>), Times.Once);
   mockstore.Verify ( m => m.Save(It.IsAny<int>), Times.Never);   
 }

public void example_2()
{
    Mock<IRepository> mockRepository = new Mock<IRepository>
    mockRepository.Setup(x => x.GetBands()).Returns(listOfBands);
    var bandController = new BandController(mockRepository.Object);
    bandController.MethodToTest();
    bandController.Should().NotBeNull();
}

//example 3
namespace TestNinje.UnitTests.Mocking
{
	[TestFixture]
	public class HouseKeeperServiceTests
		[Test]
		public void SendStatementEmails_WhenCalled_GenerateStatements()
		{
			//mock a unit of work and return a list of 
			//house keeper in memory
			
			var unitOfWork = new Mock<IUnitOfWork>();
			unitOfWork.Setup(uow => uow.Query<HouseKeeper>()).Returns (new List<HouseKeeper>
			{
				new HouseKeeper { Email="a", FullName="a", Oid=1,StatementEmailBody = "c"
			}.AddQueryable());
			
			var statementGenerator = new Mock<IStatementGenerator>();
			var emailSender = new Mock<IEmailSender>();
			var messageBox = new Mock<IXtraMEssageBox();
			
			var service = new HouseKeeperService(
				unitOfWork.Object,
				statementGenerator.Object,
				emailSender.Object,
				messageBox.Object);
				
			service.SendStatementEmails ( new DateTime(2017,1,1));
			
			statementGenerator.Verify( sg => sg.SaveStatement(1,"b", (new DateTime(2017,1,1))));
			//Verify has a second argument that indicates how many time the first argument should be called
			//Times.Never, AtLeastOnce, AtLeast, AtMost, AtMostOnce, Between, Exactly, Never, Once
			//example:
			statementGenerator.Verify( sg => sg.SaveStatement(1,"b", (new DateTime(2017,1,1))), Times.Never);
			
			}
		}
}

			



//other tutorials
//https://deanhume.com/basic-introduction-to-writing-unit-tests-with-moq-part-2/

//https://github.com/moq
//http://www.nudoq.org/#!/Projects/Moq
//http://blogs.clariusconsulting.net/kzu/linq-to-mock-moq-is-born/
//https://github.com/Moq/moq4/wiki/Quickstart
//Testing .NET Core Code with xUnit.net: Getting Started" Pluralsight course. By Jason Roberts
//Mocking With Moq. Pluralsight. Donald Belcham. Jim Cooper.






