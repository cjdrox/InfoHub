using System.Linq;
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

        [Test]
        public void TestTable()
        {
            // Setup 
            var tableOne = new Table("One");
            var tableTwo = new Table("Two").WithName("Two");
            var tableThree = new Table().WithName("Three").WithColumn<string>("ID");
            var tableFour = new Table("Blah").WithName("Four").WithColumn<string>("ID", 10);
            var tableFive = new Table("Blah").WithName("Five").WithColumn<string>("ID", 10, true);

            // Execute
            var nameOne = tableOne.TableName;
            var nameTwo = tableTwo.TableName;
            var nameThree = tableThree.TableName;
            var nameFour = tableFour.TableName;
            var nameFive = tableFive.TableName;

            // Assert
            Assert.IsNotNull(tableOne);
            Assert.IsNotNull(tableTwo);
            Assert.IsNotNullOrEmpty(nameOne);
            Assert.AreEqual("One", nameOne);
            Assert.IsNotNullOrEmpty(nameTwo);
            Assert.AreEqual("Two", nameTwo);
            Assert.IsNotNullOrEmpty(nameTwo);
            Assert.AreEqual("Three", nameThree);
            Assert.IsNotNullOrEmpty(nameFour);
            Assert.AreEqual("Four", nameFour);
            Assert.IsNotNullOrEmpty(nameFive);
            Assert.AreEqual("Five", nameFive);

            Assert.IsNotNull(tableThree.ColumnTypes);
            Assert.AreEqual(1, tableThree.ColumnTypes.Count);
            Assert.AreEqual("ID", tableThree.ColumnTypes.First().Key);
            Assert.AreEqual("String", tableThree.ColumnTypes.First().Value.Type.Name);
            Assert.IsFalse(tableThree.ColumnTypes.First().Value.IsPrimary);

            Assert.IsNotNull(tableFour.ColumnTypes);
            Assert.AreEqual(1, tableFour.ColumnTypes.Count);
            Assert.AreEqual("ID", tableFour.ColumnTypes.First().Key);
            Assert.AreEqual("String", tableFour.ColumnTypes.First().Value.Type.Name);
            Assert.AreEqual(10, tableFour.ColumnTypes.First().Value.Length);
            Assert.IsFalse(tableFour.ColumnTypes.First().Value.IsPrimary);

            Assert.IsNotNull(tableFive.ColumnTypes);
            Assert.AreEqual(1, tableFive.ColumnTypes.Count);
            Assert.AreEqual("ID", tableFive.ColumnTypes.First().Key);
            Assert.AreEqual("String", tableFive.ColumnTypes.First().Value.Type.Name);
            Assert.AreEqual(10, tableFive.ColumnTypes.First().Value.Length);
            Assert.IsTrue(tableFive.ColumnTypes.First().Value.IsPrimary);
        }
    }
}
