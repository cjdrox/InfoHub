using System;
using System.IO;
using System.Security.Cryptography;
using InfoHub.Infrastructure.Security.Types;

namespace InfoHub.Infrastructure.Security.Models
{
    public class SymmetricCryptor : Cryptor
    {
        private readonly SymmetricAlgorithm _algorithm;

        public SymmetricCryptor() : this(new AesCryptoServiceProvider())
        {
        }

        public SymmetricCryptor(SymmetricAlgorithm symmetricAlgorithm)
        {
            _algorithm = symmetricAlgorithm;
        }

        public override AlgorithmType Algorithm { 
            get { return AlgorithmType.Symmetric; }
        }

        public override SymmetricAlgorithm SymmetricAlgorithm {
            get { return _algorithm; }
        }

        public override AsymmetricAlgorithm AsymmetricAlgorithm {
            get { return null; }
        }

        public override CipherMode Mode {
            get { return _algorithm.Mode; }
            set { _algorithm.Mode = value; }
        }

        public override PaddingMode PaddingMode {
            get { return _algorithm.Padding; }
            set { _algorithm.Padding = value; }
        }

        public override byte[] IV {
            get { return _algorithm.IV; }
            set { _algorithm.IV = value; }
        }

        public override string Base64EncryptionKey {
            get { return Convert.ToBase64String(_algorithm.Key); }
            set { _algorithm.Key = CreateKey(value); }
        }

        public override string Base64DecryptionKey
        {
            get { return Convert.ToBase64String(_algorithm.Key); }
            set { _algorithm.Key = CreateKey(value); }
        }

        private byte[] CreateKey(string rawKey)
        {
            var salt = new byte[_algorithm.KeySize];

            using (var rnd = new RNGCryptoServiceProvider())
            {
                rnd.GetBytes(salt);
            }

            return new Rfc2898DeriveBytes(rawKey, salt).GetBytes(_algorithm.KeySize/8);
        }

        public override string Encrypt(string plaintext)
        {
            using (var enc = _algorithm.CreateEncryptor())
            {
                using (var memoryStream = new MemoryStream())
                {
                    using (var cryptoStream = new CryptoStream(memoryStream, enc, CryptoStreamMode.Write))
                    {
                        using (var writer = new StreamWriter(cryptoStream))
                        {
                            writer.Write(plaintext);
                        }
                        return Convert.ToBase64String(memoryStream.ToArray());
                    }
                }
            }
        }

        public override string Decrypt(string cipherText)
        {
            var cipherBytes = Convert.FromBase64String(cipherText);
            string result;

            using (var dec = _algorithm.CreateDecryptor())
            {
                using (var memoryStream = new MemoryStream(cipherBytes))
                {
                    using (var cryptoStream = new CryptoStream(memoryStream, dec, CryptoStreamMode.Read))
                    {
                        using (var reader = new StreamReader(cryptoStream))
                        {
                            result = reader.ReadToEnd();
                        }
                    }
                }
            }
            return result;
        }
    }
}
