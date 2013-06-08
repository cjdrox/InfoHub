using System.Security.Cryptography;
using InfoHub.Security.Interfaces;
using InfoHub.Security.Models;
using NUnit.Framework;

namespace InfoHub.Security.Tests
{
    [TestFixture]
    public class CryptorExtensionTestFixture
    {
        [Test]
        public void EncryptTest()
        {
            // Setup
            const string testString = "test string";
            const string expected = "TPPFvYqnUK9CWMf6zOgfKw==";
            ICryptor aes = new AES
                               {
                                   Base64EncryptionKey = "testtesttesttest",
                                   Mode = CipherMode.ECB
                               };

            // Execute
            var result = testString.Encrypt(aes);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void DecryptTest()
        {
            // Setup
            const string testString = "TPPFvYqnUK9CWMf6zOgfKw==";
            const string expected = "test string";
            ICryptor aes = new AES
            {
                Base64DecryptionKey = "testtesttesttest",
                Mode = CipherMode.ECB
            };

            // Execute
            var result = testString.Decrypt(aes);

            // Assert
            Assert.AreEqual(expected, result);
        }
    }
}
