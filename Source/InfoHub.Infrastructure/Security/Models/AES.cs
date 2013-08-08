using System.Security.Cryptography;

namespace InfoHub.Infrastructure.Security.Models
{
    public class AES : SymmetricCryptor
    {
        public AES() : base(new AesCryptoServiceProvider())
        {
        }
    }
}