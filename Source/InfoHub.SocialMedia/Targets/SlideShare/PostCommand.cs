using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

namespace InfoHub.FaceBook.Targets.SlideShare
{
    /// <summary>
    /// Big thanks to Brian Grinstead: http://www.briangrinstead.com/blog/multipart-form-post-in-c
    /// </summary>
    public static class PostCommand
    {
        
        private static readonly Encoding Encoding = Encoding.UTF8;
        public static string Execute(string postUrl, Dictionary<string, object> postParameters)
        {
            const string formDataBoundary = "-----------------------------28947758029299";
            const string contentType = "multipart/form-data; boundary=" + formDataBoundary;

            byte[] formData = GetMultipartFormData(postParameters, formDataBoundary);
            string responseXml;
            using (var response = PostForm(postUrl, contentType, formData))
            {
                var reader = new StreamReader(response.GetResponseStream());

                responseXml = reader.ReadToEnd();
            }

            return responseXml;
        }
        private static HttpWebResponse PostForm(string postUrl, string contentType, byte[] formData)
        {
            var request = WebRequest.Create(postUrl) as HttpWebRequest;

            if (request == null)
            {
                throw new NullReferenceException("request is not a http request");
            }

            // Set up the request properties
            request.Method = "POST";
            request.ContentType = contentType;
            request.CookieContainer = new CookieContainer();
            request.ContentLength = formData.Length;  // We need to count how many bytes we're sending. 

            using (Stream requestStream = request.GetRequestStream())
            {
                // Push it out there
                requestStream.Write(formData, 0, formData.Length);
                requestStream.Close();
            }

            return request.GetResponse() as HttpWebResponse;
        }

        private static byte[] GetMultipartFormData(Dictionary<string, object> postParameters, string boundary)
        {
            Stream formDataStream = new MemoryStream();

            foreach (var param in postParameters)
            {
                if (param.Value is FileParameter)
                {
                    var fileToUpload = (FileParameter)param.Value;

                    // Add just the first part of this param, since we will write the file data directly to the Stream
                    string header = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"; filename=\"{2}\";\r\nContent-Type: {3}\r\n\r\n",
                        boundary,
                        param.Key,
                        fileToUpload.FileName ?? param.Key,
                        fileToUpload.ContentType ?? "application/octet-stream");

                    formDataStream.Write(Encoding.GetBytes(header), 0, header.Length);

                    // Write the file data directly to the Stream, rather than serializing it to a string.
                    formDataStream.Write(fileToUpload.File, 0, fileToUpload.File.Length);
                }
                else
                {
                    var postData = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}\r\n",
                        boundary,
                        param.Key,
                        HttpUtility.UrlEncode(param.Value.ToString()));
                    formDataStream.Write(Encoding.GetBytes(postData), 0, postData.Length);
                }
            }

            // Add the end of the request
            var footer = "\r\n--" + boundary + "--\r\n";
            formDataStream.Write(Encoding.GetBytes(footer), 0, footer.Length);

            // Dump the Stream into a byte[]
            formDataStream.Position = 0;
            var formData = new byte[formDataStream.Length];
            formDataStream.Read(formData, 0, formData.Length);
            formDataStream.Close();

            return formData;
        }

        public class FileParameter
        {
            public byte[] File { get; set; }
            public string FileName { get; set; }
            public string ContentType { get; set; }
            public FileParameter(byte[] file) : this(file, null) { }
            public FileParameter(byte[] file, string filename) : this(file, filename, null) { }
            public FileParameter(byte[] file, string filename, string contenttype)
            {
                File = file;
                FileName = filename;
                ContentType = contenttype;
            }
        }
    }
}