using System.Collections.Generic;
using System.IO;
using System.Net;

namespace InfoHub.FaceBook.Targets.SlideShare
{
    public static class GetCommand
    {
        public static string Execute(string url, ICollection<KeyValuePair<string, object>> parameters)
        {
            var request = WebRequest.Create(url + "?" + Helper.CreateFormattedRequest(parameters)) as HttpWebRequest;
            string responseXml;

            using (var response = request.GetResponse() as HttpWebResponse)
            {
                var reader = new StreamReader(response.GetResponseStream());
                responseXml = reader.ReadToEnd();
            }

            return responseXml;
        }
    }
}
