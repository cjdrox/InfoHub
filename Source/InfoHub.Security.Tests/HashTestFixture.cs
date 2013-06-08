using InfoHub.Security.Models;
using NUnit.Framework;

namespace InfoHub.Security.Tests
{
    [TestFixture]
    public class HashTestFixture
    {
        [Test]
        public void MD5HashTest()
        {
            // Setup
            const string testString = "test string";
            const string expected = "6f8db599de986fab7a21625b7916589c";

            // Execute
            var result = testString.MD5Hash().ToLower();

            // Assert
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void SHA1HashTest()
        {
            // Setup
            const string testString = "test string";
            const string expected = "661295c9cbf9d6b2f6428414504a8deed3020641";

            // Execute
            var result = testString.SHA1Hash().ToLower();

            // Assert
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void ROT13Test()
        {
            // Setup
            const string testString = "Lo, A Test String";
            const string expected = "Yb, N Grfg Fgevat";

            // Execute
            var result = testString.ROT13();

            // Assert
            Assert.AreEqual(expected, result);
        }
    }
}
