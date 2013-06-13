using InfoHub.ORM.Interfaces;
using InfoHub.ORM.Models;
using NUnit.Framework;

namespace InfoHub.ORM.Tests
{
    [TestFixture]
    public class ORMTestFixture
    {
        [Test]
        public void TestConfiguration()
        {
            // Setup
            IConfiguration configuration = new Configuration("localhost", "blah", "3308", "sa", "sa");

            // Execute
            var host = configuration.Host;
            var database = configuration.Database;
            var port = configuration.Port;
            var user = configuration.Username;
            var pass = configuration.Password;

            var constring = string.Format("SERVER={0}; DATABASE={1}; UID= {2}; PASSWORD={3};",
                    host, database, user, pass);

            // Assert
            Assert.IsNotNull(configuration);
            Assert.AreEqual("localhost", host);
            Assert.AreEqual("blah", database);
            Assert.AreEqual("3308", port);
            Assert.AreEqual("sa", user);
            Assert.AreEqual("sa", pass);
            Assert.AreEqual(constring, configuration.ConnectionString);
            Assert.AreEqual(true, configuration.IsValid);

            // Invalidate and assert
            configuration.Dispose();
            Assert.AreEqual(false, configuration.IsValid);
        }
    }
}
