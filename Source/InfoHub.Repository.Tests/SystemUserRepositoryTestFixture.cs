using InfoHub.Entity.Entities;
using InfoHub.Infrastructure.Security.Helpers;
using InfoHub.ORM.Interfaces;
using InfoHub.ORM.Models;
using InfoHub.ORM.Services;
using Infohub.Repository.Repositories;
using NUnit.Framework;

namespace InfoHub.Repository.Tests
{
    [TestFixture]
    public class SystemUserRepositoryTestFixture
    {
        
        [Test]
        public void SystemUserWriteTest()
        {
            IConfiguration configuration = new Configuration("localhost", "blah", "3308", "root", "");
            using (var adapter = new MySQLAdapter(configuration, true))
            {

            }

            // Setup 

            var user = new SystemUser
                           {
                               Username = "test",
                               Passhash = "password".Hash()
                           };

            // Execute
            //repository.Add(user);
            //var test = repository.GetUserByUsername("test");

            // Assert
            //Assert.IsNotNull(test);
        }
    }
}
