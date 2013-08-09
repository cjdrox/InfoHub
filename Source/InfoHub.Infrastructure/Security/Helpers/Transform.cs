using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using InfoHub.Infrastructure.Security.Interfaces;
using InfoHub.Infrastructure.Security.Models;
using HashAlgorithm = InfoHub.Infrastructure.Security.Types.HashAlgorithm;

namespace InfoHub.Infrastructure.Security.Helpers
{
    public static class Transform
    {
        private static readonly X509Store Store = Store ?? new X509Store();
        private static X509Certificate2 _certificate = _certificate ?? GetCertificate();

        private static X509Certificate2 GetCertificate()
        {
            Store.Open(OpenFlags.ReadOnly);
            var certs = Store.Certificates.Find(X509FindType.FindBySubjectName, Properties.Settings.Default.CertificateSubject, false);

            _certificate = certs[0];
            Store.Close();
            return _certificate;
        }

        public static string ROT13(this string str)
        {
            var array = str.ToCharArray();
            for (var i = 0; i < array.Length; i++)
            {
                int number = array[i];

                if (number >= 'a' && number <= 'z')
                {
                    if (number > 'm') { number -= 13;}
                    else { number += 13; }
                }
                else if (number >= 'A' && number <= 'Z')
                {
                    if (number > 'M') { number -= 13; }
                    else { number += 13; }
                }

                array[i] = (char)number;
            }
            return new string(array);
        }

        public static string Encrypt(this string str, ICryptor cryptor)
        {
            return cryptor.Encrypt(str);
        }

        public static string Encrypt(this string plainText)
        {
            return RSAWrapper.Encrypt(plainText, _certificate);
        }

        public static string Decrypt(this string cipherText)
        {
            return RSAWrapper.Decrypt(cipherText, _certificate);
        }

        public static string Decrypt(this string str, ICryptor cryptor)
        {
            return cryptor.Decrypt(str);
        }

        public static string MD5Hash(this string str)
        {
            var md5 = MD5.Create();
            

            var inputBytes = Encoding.ASCII.GetBytes(str);
            var hash = md5.ComputeHash(inputBytes);

            return hash.ToHexString();
        }

        public static string Hash(this string str, HashAlgorithm algorithm = HashAlgorithm.SHA512)
        {
            var inputBytes = Encoding.ASCII.GetBytes(str);

            System.Security.Cryptography.HashAlgorithm hashAlgorithm = new SHA512CryptoServiceProvider();

            switch (algorithm)
            {
                case HashAlgorithm.MD5: hashAlgorithm = new MD5CryptoServiceProvider();
                    break;
                case HashAlgorithm.SHA1: hashAlgorithm = new SHA1CryptoServiceProvider(); 
                    break;
                case HashAlgorithm.SHA256: hashAlgorithm = new SHA256CryptoServiceProvider();
                    break;
                case HashAlgorithm.SHA384: hashAlgorithm = new SHA384CryptoServiceProvider();
                    break;
                case HashAlgorithm.SHA512: hashAlgorithm = new SHA512CryptoServiceProvider();
                    break;
            }

            var result = hashAlgorithm.ComputeHash(inputBytes);

            return result.ToHexString();
        }

        public static string ToHexString(this byte[] bytes)
        {
            var sb = new StringBuilder();

            for (var i = 0; i < bytes.Length; i++)
            {
                sb.Append(bytes[i].ToString("X2"));
            }

            return sb.ToString();
        }
    }
}
