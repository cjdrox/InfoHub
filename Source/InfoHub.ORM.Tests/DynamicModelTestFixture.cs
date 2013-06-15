using InfoHub.ORM.Interfaces;
using InfoHub.ORM.Models;
using NUnit.Framework;

namespace InfoHub.ORM.Tests
{
    [TestFixture]
    public class DynamicModelTestFixture
    {
        [Test]
        public void TestImplicitCreation()
        {
            // Setup
            IConfiguration configuration = new Configuration("localhost", "blah", "3308", "root", "");

            // Execute
            TransactedModel model = new Test();
            var dead = model.OpenConnection();
            var live = model.OpenConnection(configuration);
            var mock = DynamicModel.Open(configuration);

            // Assert
            Assert.IsNotNull(live);
            Assert.IsNull(dead);
            Assert.IsNotNull(mock);
            Assert.IsNotNull(mock.OpenConnection());
        }

        [Test]
        public void TestExplicitCreation()
        {
            // Setup
            IConfiguration configuration = new Configuration("localhost", "blah", "3308", "root", "");

            // Execute
            TransactedModel model = new Table("Test");
            var dead = model.OpenConnection();
            var live = model.OpenConnection(configuration);
            var mock = DynamicModel.Open(configuration);

            // Assert
            Assert.IsNotNull(live);
            Assert.IsNull(dead);
            Assert.IsNotNull(mock);
            Assert.IsNotNull(mock.OpenConnection());
        }

        internal class Test : Table
        {
            
        }
    }
}
