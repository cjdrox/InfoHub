using InfoHub.Business.Interfaces;
using InfoHub.Entity.Interfaces;
using InfoHub.Entity.Repositories;
using Moq;
using NUnit.Framework;

namespace InfoHub.Business.Test.AccountProfileService
{
    [TestFixture]
    public class AccountProfileServiceFixture
    {
        private IAccountProfileService _accountProfileService;
        private Mock _mockrepository;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            _mockrepository = new Mock<IAccountProfileRepository>().As<IAccountProfileRepository>();
            _accountProfileService = new Services.AccountProfileService(new AccountProfileRepository());
        }

        [SetUp]
        public void SetupContext()
        {
        }

        [TearDown]
        public void TearDown()
        {
            
        }

        [Test]
        public void GetByUsername()
        {
            //Setup
            
            //Execute
            var username = _accountProfileService.GetByUsername("admin@infohub.com");

            //Verify
            _mockrepository.VerifyAll();

            // Assert
            Assert.IsNotNull(username);
            Assert.IsNotNull(username.ServiceData);
        }
    }
}
