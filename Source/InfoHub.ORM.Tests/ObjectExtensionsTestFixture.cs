using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using InfoHub.ORM.Extensions;
using NUnit.Framework;
using System.Collections.Specialized;

namespace InfoHub.ORM.Tests
{
    [TestFixture]
    public class ObjectExtensionsTestFixture
    {
        [Test]
        public void ToDictionary()
        {
            // Setup
            var test1 = new Test();
            var test2 = new NameValueCollection {{"test", "one"}};

            // Execute
            var result1 = test1.ToDictionary();
            var result2 = test2.ToDictionary();

            // Assert
            Assert.IsNotNull(result1);
            Assert.AreEqual(2, result1.Count());

            Assert.IsNotNull(result2);
            Assert.AreEqual(1, result2.Count());
        }

        [Test]
        public void DBCommandExtensions()
        {
            // Setups
            DbCommand command = new SqlCommand("Help");

            // Execute
            command.AddParams(new object[] {"Test"});
            var num = command.Parameters.Count;

            // Assert
            Assert.IsNotNull(command);
            Assert.AreEqual(1, num);
        }

        internal class Test
        {
            public string Name { get; set; }
            public int ID { get; set; }
        }
    }
}
