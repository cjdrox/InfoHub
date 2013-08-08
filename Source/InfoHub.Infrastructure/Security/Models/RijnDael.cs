using System.Security.Cryptography;

namespace InfoHub.Infrastructure.Security.Models
{
    public class RijnDael : SymmetricCryptor
    {
        public RijnDael() : base(new RijndaelManaged())
        {
        }
    }
}
