using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using InfoHub.Security.Types;

namespace InfoHub.Security.Models
{
    public class AES : Cryptor
    {
        private readonly SymmetricAlgorithm _aes;

        public AES() : this(new AesCryptoServiceProvider())
        {
        }

        private AES(SymmetricAlgorithm symmetricAlgorithm)
        {
            _aes = symmetricAlgorithm;
        }

        public override AlgorithmType Algorithm { 
            get { return AlgorithmType.Symmetric; }
        }

        public override SymmetricAlgorithm SymmetricAlgorithm {
            get { return _aes; }
        }

        public override AsymmetricAlgorithm AsymmetricAlgorithm {
            get { return null; }
        }

        public override CipherMode Mode {
            get { return _aes.Mode; }
            set { _aes.Mode = value; }
        }

        public override PaddingMode PaddingMode {
            get { return _aes.Padding; }
            set { _aes.Padding = value; }
        }

        public override byte[] IV {
            get { return _aes.IV; }
            set { _aes.IV = value; }
        }

        public override string Base64EncryptionKey {
            get { return Encoding.UTF8.GetString(_aes.Key); }
            set { _aes.Key = Encoding.UTF8.GetBytes(value); }
        }

        public override string Base64DecryptionKey {
            get { return Encoding.UTF8.GetString(_aes.Key); }
            set { _aes.Key = Encoding.UTF8.GetBytes(value); }
        }

        public override string Encrypt(string plaintext)
        {
            using (var enc = _aes.CreateEncryptor())
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
            using (var dec = _aes.CreateDecryptor())
            {
                using (var memoryStream = new MemoryStream(cipherBytes))
                {
                    using (var cryptoStream = new CryptoStream(memoryStream, dec, CryptoStreamMode.Read))
                    {
                        string result;
                        using (var reader = new StreamReader(cryptoStream))
                        {
                            result = reader.ReadToEnd();
                        }
                        return result;
                    }
                }
            }
        }
    }
}
