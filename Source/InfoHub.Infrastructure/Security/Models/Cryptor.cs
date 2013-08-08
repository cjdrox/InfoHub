using System.Security.Cryptography;
using InfoHub.Infrastructure.Security.Interfaces;
using InfoHub.Infrastructure.Security.Types;

namespace InfoHub.Infrastructure.Security.Models
{
    public abstract class Cryptor : ICryptor
    {
        public virtual AlgorithmType Algorithm { 
            get { return AlgorithmType.Undefined;}
        }

        public abstract SymmetricAlgorithm SymmetricAlgorithm { get; }
        public abstract AsymmetricAlgorithm AsymmetricAlgorithm { get; }
        public virtual CipherMode Mode { get; set; }
        public virtual PaddingMode PaddingMode { get; set; }
        public virtual byte[] IV { get; set; }
        public virtual byte[] Salt { get; set; }
        public virtual string Base64EncryptionKey { get; set; }
        public virtual string Base64DecryptionKey { get; set; }
        public abstract string Encrypt(string plaintext);
        public abstract string Decrypt(string plaintext);
    }
}