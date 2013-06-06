using System.Security.Cryptography;
using InfoHub.Security.Interfaces;
using InfoHub.Security.Types;

namespace InfoHub.Security.Models
{
    public abstract class Cryptor : ICryptor
    {
        public virtual AlgorithmType Algorithm { 
            get { return AlgorithmType.Undefined;}
        }
        public virtual SymmetricAlgorithm SymmetricAlgorithm {
            get { return null; }
        }
        public virtual AsymmetricAlgorithm AsymmetricAlgorithm {
            get { return null; }
        }
        public virtual CipherMode Mode { get; set; }
        public virtual PaddingMode PaddingMode { get; set; }
        public virtual byte[] IV { get; set; }
        public virtual string Base64EncryptionKey { get; set; }
        public virtual string Base64DecryptionKey { get; set; }

        public virtual string Encrypt(string plaintext)
        {
            throw new System.NotImplementedException();
        }

        public virtual string Decrypt(string plaintext)
        {
            throw new System.NotImplementedException();
        }
    }
}