using System;
using InfoHub.Security.Interfaces;

namespace InfoHub.Security.Models
{
    public static class Transform
    {
        public static string ROT13(this string str)
        {
            throw new NotImplementedException();
        }

        public static string Encrypt(this string str, ICryptor cryptor)
        {
            return cryptor.Encrypt(str);
        }

        public static string Decrypt(this string str, ICryptor cryptor)
        {
            return cryptor.Decrypt(str);
        }

        public static string Hash(this string str)
        {
            throw new NotImplementedException();
        }
    }
}
