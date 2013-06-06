using System.Security.Cryptography;
using InfoHub.Security.Types;

namespace InfoHub.Security.Interfaces
{
    public interface ICryptor
    {
        AlgorithmType Algorithm { get; }
        SymmetricAlgorithm SymmetricAlgorithm { get; }
        AsymmetricAlgorithm AsymmetricAlgorithm { get; }
        CipherMode Mode { get; set; }
        PaddingMode PaddingMode { get; set; }
        byte[] IV { get; set; }
        string Base64EncryptionKey { get; set; }
        string Base64DecryptionKey { get; set; }
        string Encrypt(string plaintext);
        string Decrypt(string plaintext);
    }
}