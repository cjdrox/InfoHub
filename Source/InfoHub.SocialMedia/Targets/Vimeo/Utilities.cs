using System;
using System.Text;
using System.Security.Cryptography;
using System.Collections;
using System.Xml;
using InfoHub.SocialMedia.Targets.Vimeo.Objects;

namespace InfoHub.SocialMedia.Targets.Vimeo
{
    public class Utilities
    {
        internal string GenerateMD5(string inputString)
        {
            MD5 md5Algo = MD5.Create();
            byte[] b = Encoding.UTF8.GetBytes(inputString);
            byte[] bMd5 = md5Algo.ComputeHash(b);
            StringBuilder hex = new StringBuilder(b.Length * 2);

            foreach (byte bi in bMd5)
            {
                hex.AppendFormat("{0:x2}", bi);
            }

            return hex.ToString();
        }

        internal string GetMethodURI(string baseURI, SortedList parms, Authentication auth)
        {
            parms[Parameters.APIKEY] = auth.PublicKey;

            StringBuilder signature = new StringBuilder();
            signature.Append(auth.SecretKey);

            foreach (string k in parms.Keys)
            {
                signature.Append(k);
                signature.Append(parms[k]);
            }

            string apiSignature = GenerateMD5(signature.ToString());

            StringBuilder URI = new StringBuilder();
            URI.Append(baseURI);

            Boolean first = true;
            foreach (string k in parms.Keys)
            {
                if (first)
                {
                    first = false;
                    URI.Append("?");
                }
                else
                {
                    URI.Append("&");
                }
                URI.Append(k);
                URI.Append("=");
                URI.Append(parms[k]);
            }

            URI.Append("&api_sig=");
            URI.Append(apiSignature);

            return URI.ToString();
        }

        public XmlDocument GenerateUploadManifest(string hash, XmlDocument AppendTo)
        {
            if (AppendTo == null)
            {
                AppendTo = new XmlDocument();
                XmlDeclaration Dec = AppendTo.CreateXmlDeclaration("1.0", "utf-8", String.Empty);
                XmlElement mainNode = AppendTo.CreateElement("files");
                XmlElement child = AppendTo.CreateElement("file");
                XmlAttribute md5 = AppendTo.CreateAttribute("md5");
                md5.Value = hash;
                child.Attributes.Append(md5);
                mainNode.AppendChild(child);

                XmlElement root = AppendTo.DocumentElement;
                AppendTo.InsertBefore(Dec, root);
                AppendTo.AppendChild(mainNode);
            }
            else
            {
                XmlNode child = AppendTo.CreateNode(XmlNodeType.Element, "file", String.Empty);
                XmlAttribute md5 = AppendTo.CreateAttribute("md5");
                md5.Value = hash;
                child.Attributes.Append(md5);
                AppendTo.ChildNodes[1].AppendChild(child);
            }

            return AppendTo;
        }
    }
}