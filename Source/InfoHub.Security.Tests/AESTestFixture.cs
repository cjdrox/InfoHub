using System.Security.Cryptography;
using System.Text;
using InfoHub.Security.Interfaces;
using InfoHub.Security.Models;
using InfoHub.Security.Types;
using NUnit.Framework;

namespace InfoHub.Security.Tests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestFixture]
    public class AESTestFixture
    {
        private ICryptor _cryptor;

        #region Setup and Teardown Code

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            _cryptor = new AES();
        }

        [SetUp]
        public void SetUp()
        {
        }

        [TearDown]
        public void TearDown()
        {
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
        }

        #endregion

        [Test]
        public void AESTestEnKey()
        {
            // Setup
            _cryptor.Base64EncryptionKey = "testtesttesttest";

            // Execute
            var dekey = _cryptor.Base64DecryptionKey;

            // Assert
            Assert.IsNotNull(dekey);
            Assert.AreEqual("testtesttesttest", dekey);
        }

        [Test]
        public void AESTestDeKey()
        {
            // Setup
            _cryptor.Base64DecryptionKey = "testtesttesttest";

            // Execute
            var enkey = _cryptor.Base64EncryptionKey;

            // Assert
            Assert.IsNotNull(enkey);
            Assert.AreEqual("testtesttesttest", enkey);
        }

        [Test]
        public void AESTestSymmetry()
        {
            // Setup

            // Execute
            var iv = Encoding.UTF8.GetBytes("testtesttesttest");
            var algo = _cryptor.Algorithm;
            _cryptor.IV = iv;
            _cryptor.PaddingMode = PaddingMode.PKCS7;
            _cryptor.Mode = CipherMode.ECB;

            // Assert
            Assert.AreEqual(AlgorithmType.Symmetric, algo);
            Assert.AreEqual(_cryptor.SymmetricAlgorithm.GetType().Name, "AesCryptoServiceProvider");
            Assert.IsNull(_cryptor.AsymmetricAlgorithm);
            Assert.AreEqual(iv, _cryptor.IV);
            Assert.AreEqual(PaddingMode.PKCS7, _cryptor.PaddingMode);
            Assert.AreEqual(CipherMode.ECB, _cryptor.Mode);
        }

        [Test]
        public void AESEncryptNullString()
        {
            // Setup
            const string testString = null;
            _cryptor.Base64EncryptionKey = "testtesttesttest";
            _cryptor.Mode = CipherMode.ECB;

            // Execute
            var result = _cryptor.Encrypt(testString);

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public void AESEncryptWithCBC()
        {
            // Setup
            const string testString = "test";
            const string expected = @"bjt1UATPu/dzzr1IO1ayXw==";

            _cryptor.Base64EncryptionKey = "testtesttesttest";
            _cryptor.IV = Encoding.UTF8.GetBytes("testtesttesttest");
            _cryptor.Mode = CipherMode.CBC;

            // Execute
            var result = _cryptor.Encrypt(testString);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void AESDecryptWithCBC()
        {
            // Setup
            const string expected = "test";
            const string testString = @"bjt1UATPu/dzzr1IO1ayXw==";

            _cryptor.Base64DecryptionKey = "testtesttesttest";
            _cryptor.IV = Encoding.UTF8.GetBytes("testtesttesttest");
            _cryptor.Mode = CipherMode.CBC;

            // Execute
            var result = _cryptor.Decrypt(testString);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void AESEncryptWithECB()
        {
            // Setup
            const string testString = "test";
            const string expected = @"1DuUd1pIMjeETuUqmGL4fw==";

            _cryptor.Base64EncryptionKey = "testtesttesttest";
            _cryptor.Mode = CipherMode.ECB;

            // Execute
            var result = _cryptor.Encrypt(testString);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void AESDecryptWithECB()
        {
            // Setup
            const string expected = "test";
            const string testString = @"1DuUd1pIMjeETuUqmGL4fw==";

            _cryptor.Base64DecryptionKey = "testtesttesttest";
            _cryptor.Mode = CipherMode.ECB;

            // Execute
            var result = _cryptor.Decrypt(testString);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expected, result);
        }
    }
}
