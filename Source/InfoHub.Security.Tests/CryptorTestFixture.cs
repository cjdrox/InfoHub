using System;
using System.Security.Cryptography;
using System.Text;
using InfoHub.Security.Interfaces;
using InfoHub.Security.Models;
using InfoHub.Security.Types;
using NUnit.Framework;

namespace InfoHub.Security.Tests
{
    [TestFixture]
    public class CryptorTestFixture
    {
        private ICryptor _cryptor;

        internal class CryptorImpl : Cryptor
        {
        }

        #region Setup and Teardown Code

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            _cryptor = new CryptorImpl();
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
        public void TestProperties()
        {
            // Setup

            //Execute
            var algo = _cryptor.Algorithm;
            var symm = _cryptor.SymmetricAlgorithm;
            var asym = _cryptor.AsymmetricAlgorithm;

            // Assert
            Assert.AreEqual(AlgorithmType.Undefined, algo);
            Assert.IsNull(symm);
            Assert.IsNull(asym);
        }

        [Test]
        public void TestSymmetry()
        {
            // Setup

            // Execute
            var iv = Encoding.UTF8.GetBytes("testtesttesttest");
            _cryptor.IV = iv;
            _cryptor.PaddingMode = PaddingMode.PKCS7;
            _cryptor.Mode = CipherMode.ECB;

            // Assert
            Assert.AreEqual(iv, _cryptor.IV);
            Assert.AreEqual(PaddingMode.PKCS7, _cryptor.PaddingMode);
            Assert.AreEqual(CipherMode.ECB, _cryptor.Mode);
        }

        [Test]
        public void TestEcxeptions()
        {
            // Setup
            const string testString = "test";

            // Execute
            
            // Assert
            Assert.Throws<NotImplementedException>(() => _cryptor.Encrypt(testString));
            Assert.Throws<NotImplementedException>(() => _cryptor.Decrypt(testString));
        }
    }
}
