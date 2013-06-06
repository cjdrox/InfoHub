using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace InfoHub.FaceBook.Targets.SlideShare
{
    public static class Helper
    {
        public static string GetYorN(bool boolean)
        {
            return boolean ? "Y" : "N";
        }

        public static string CalculateSHA1(string text, Encoding enc)
        {
            var buffer = enc.GetBytes(text);
            var cryptoTransformSHA1 = new SHA1CryptoServiceProvider();

            return BitConverter.ToString(cryptoTransformSHA1.ComputeHash(buffer)).Replace("-", "").ToLower();
        }

        public static string CreateFormattedRequest(ICollection<KeyValuePair<string, object>> values)
        {
            var parameterBuilder = new StringBuilder();
            var counter = 0;
            foreach (var value in values)
            {
                parameterBuilder.AppendFormat("{0}={1}", value.Key, HttpUtility.UrlEncode(value.Value.ToString()));

                if (counter != values.Count - 1)
                {
                    parameterBuilder.Append("&");
                }

                counter++;
            }

            return parameterBuilder.ToString();
        }
    }
}
