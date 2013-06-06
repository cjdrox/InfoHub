using System;
using System.IO;
using System.Security.Cryptography;
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
            get { return Convert.ToBase64String(_aes.Key); }
            set { _aes.Key = Convert.FromBase64String(value); }
        }

        public override string Base64DecryptionKey {
            get { return Convert.ToBase64String(_aes.Key); }
            set { _aes.Key = Convert.FromBase64String(value); }
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

        public override string Decrypt(string plaintext)
        {
            using (var enc = _aes.CreateDecryptor())
            {
                using (var memoryStream = new MemoryStream())
                {
                    using (var cryptoStream = new CryptoStream(memoryStream, enc, CryptoStreamMode.Write))
                    {
                        using (var reader = new StreamReader(cryptoStream))
                        {
                            reader.ReadToEnd();
                        }
                        return Convert.ToBase64String(memoryStream.ToArray());
                    }
                }
            }
        }
    }
}
