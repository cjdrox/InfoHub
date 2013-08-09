using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace InfoHub.Infrastructure.Security.Models
{
    public class RSAWrapper
    {
        public static string Encrypt(string input, X509Certificate2 certificate)
        {
            var ptBytes = Encoding.Unicode.GetBytes(input);
            var provider = (RSACryptoServiceProvider) certificate.PublicKey.Key;
            return Convert.ToBase64String(provider.Encrypt(ptBytes, false));
        }

        public static string Decrypt(string input, X509Certificate2 certificate)
        {
            var ptBytes = Encoding.Unicode.GetBytes(input);
            var provider = (RSACryptoServiceProvider)certificate.PrivateKey;
            return Convert.ToBase64String(provider.Decrypt(ptBytes, false));
        }
    }
}
