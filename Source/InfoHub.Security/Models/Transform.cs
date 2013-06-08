using System.Security.Cryptography;
using System.Text;
using InfoHub.Security.Interfaces;

namespace InfoHub.Security.Models
{
    public static class Transform
    {
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

        public static string SHA1Hash(this string str)
        {
            var inputBytes = Encoding.ASCII.GetBytes(str);
            
            SHA1 sha = new SHA1CryptoServiceProvider();

            var result = sha.ComputeHash(inputBytes);

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
