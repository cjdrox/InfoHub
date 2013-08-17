using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Collections;
using System.Net;
using System.IO;
using System.Linq;
using InfoHub.SocialMedia.Targets.Vimeo.Objects;

namespace InfoHub.SocialMedia.Targets.Vimeo
{
    public class Methods
    {
        #region Vimeo Advanced API Methods

        private const string auth_getFrob = "vimeo.auth.getFrob";
        private const string auth_getToken = "vimeo.auth.getToken";
        private const string videos_getUploadTicket = "vimeo.videos.upload.getTicket";
        private const string videos_verifyManifest = "vimeo.videos.upload.verifyManifest";
        private const string videos_setPrivacy = "vimeo.videos.setPrivacy";
        private const string videos_setTitle = "vimeo.videos.setTitle";
        private const string videos_setDescription = "vimeo.videos.setDescription";
        private const string videos_getInfo = "vimeo.videos.getInfo";
        private const string videos_getPresets = "vimeo.videos.embed.getPresets";
        private const string videos_setPreset = "vimeo.videos.embed.setPreset";
        private const string videos_getQuota = "vimeo.videos.upload.getQuota";
        private const string videos_confirm = "vimeo.videos.upload.confirm";

        #endregion

        private const string TokenError = "You need a valid token to perform this operation";

        private readonly Authentication _auth;
        private readonly Utilities _utils = new Utilities();

        public Methods(Authentication auth)
        {
            _auth = auth;
        }

        #region Video

        public string FileUpload(string filePath, UploadTicket UT)
        {
            Validate();

            var sl = new SortedList {{Parameters.TICKETID, UT.Ticket}, {Parameters.AUTHTOKEN, _auth.Token}};

            UT.URI = UT.URI.Remove(UT.URI.IndexOf('?'));
            var url = _utils.GetMethodURI(UT.URI, sl, _auth);

            // create the request
            var request = WebRequest.Create(url) as HttpWebRequest;

            // get a boundary string - used to separate parts of the form data
            var boundary = String.Format("----------{0:B}", Guid.NewGuid());

            // set the content type and method
            request.ContentType = String.Format("multipart/form-data; boundary={0}", boundary);
            request.Method = "POST";
            request.KeepAlive = true;
            request.Credentials = System.Net.CredentialCache.DefaultCredentials;

            // build the form part header
            var header = new StringBuilder();
            header.AppendFormat("--{0}\r\n", boundary);
            header.AppendFormat("Content-Disposition: form-data; name=\"file_data\"; filename=\"{0}\"\r\n", Path.GetFileName(filePath), filePath);
            header.Append("Content-Type: application/octet-stream\r\n\r\n");

            // get the header as bytes
            var headerBytes = Encoding.ASCII.GetBytes(header.ToString());

            // read the file
            var fileBytes = File.ReadAllBytes(filePath);

            // get the footer bytes
            byte[] footerBytes = Encoding.ASCII.GetBytes(String.Format("\r\n--{0}--", boundary));
            
            // get the complete set of bytes
            byte[] data = headerBytes
            .Concat(fileBytes)
            .Concat(footerBytes).ToArray();

            // set the content length
            request.ContentLength = data.Length;

            // write the bytes to the request stream
            using (Stream s = request.GetRequestStream())
            {
                s.Write(data, 0, data.Length);
            }

            // get the response
            var response = request.GetResponse() as HttpWebResponse;

            Stream responseStream = response.GetResponseStream();
            var responseReader = new StreamReader(responseStream);

            return responseReader.ReadToEnd();
        }

        public string ConfirmUpload(string ticketId, XmlDocument XMLUploadManifest)
        {
            Validate();

            var xd = new XmlDocument();
            if (SendXMLManifest(videos_confirm, xd, XMLUploadManifest.InnerXml, Parameters.TICKETID, ticketId))
            {
                return xd.DocumentElement.SelectSingleNode("ticket").Attributes["video_id"].Value;
            }
            else
            {
                return null;
            }
        }

        public string VerifyUploadManifest(string ticketId, XmlDocument XMLUploadManifest)
        {
            Validate();

            var xd = new XmlDocument();
            if (SendXMLManifest(videos_verifyManifest, xd, XMLUploadManifest.InnerXml, Parameters.TICKETID, ticketId))
            {
                int MissingFiles = xd.DocumentElement.SelectSingleNode("missing_files").ChildNodes.Count;
                if (MissingFiles > 0)
                {
                    throw new Exception("There are " + MissingFiles + " files missing in the manifest");
                }
                else
                {
                    return xd.DocumentElement.SelectSingleNode("ticket").Attributes["md5"].Value;
                }
            }
            else
            {
                return null;
            }
        }

        public bool SetVideoPrivacy(string videoId, bool visible)
        {
            Validate();

            var xd = new XmlDocument();
            if (CallMethod(videos_setPrivacy, xd, Parameters.PRIVACY, visible ? "anybody" : "disable", Parameters.VIDEOID, videoId))
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public bool SetVideoTitle(string videoId, string title)
        {
            Validate();

            XmlDocument xd = new XmlDocument();
            if (CallMethod(videos_setTitle, xd, Parameters.VTITLE, title, Parameters.VIDEOID, videoId))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool SetVideoDescription(string videoId, string description)
        {
            Validate();

            XmlDocument xd = new XmlDocument();
            if (CallMethod(videos_setDescription, xd, Parameters.VDESCRIPTION, description, Parameters.VIDEOID, videoId))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool SetVideoPreset(string videoId, string presetId)
        {
            Validate();

            XmlDocument xd = new XmlDocument();
            if (CallMethod(videos_setPreset, xd, Parameters.PRESETID, presetId, Parameters.VIDEOID, videoId))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public Video GetVideoInfo(string videoId)
        {
            Video vid = new Video();
            XmlDocument xd = new XmlDocument();
            if (CallMethod(videos_getInfo, xd, Parameters.VIDEOID, videoId))
            {
                vid.VideoId = videoId;
                vid.Title = xd.DocumentElement.FirstChild["title"].InnerXml;
                vid.Description = xd.DocumentElement.FirstChild["caption"].InnerXml;
                vid.UploadDate = xd.DocumentElement.FirstChild["upload_date"].InnerXml;
                vid.Duration = xd.DocumentElement.FirstChild["duration"].InnerXml;

                XmlNodeList xNL = xd.DocumentElement.SelectSingleNode("video/thumbnails").ChildNodes;
                vid.Thumbnails = new string[xNL.Count];
                int i = 0;
                foreach (XmlNode n in xNL)
                {
                    vid.Thumbnails[i] = n.InnerXml;
                    i++;
                }

                return vid;
            }
            else
            {
                return null;
            }
        }

        public Quota GetUserQuota()
        {
            Validate();

            Quota quota = new Quota();
            XmlDocument xd = new XmlDocument();
            if (CallMethod(videos_getQuota, xd))
            {
                quota.IsUserPlus = Convert.ToBoolean(xd.DocumentElement.GetAttribute("is_plus"));
                quota.Free = Convert.ToInt32(xd.DocumentElement.FirstChild["upload_space"].Attributes["free"].Value);
                quota.Max = Convert.ToInt32(xd.DocumentElement.FirstChild["upload_space"].Attributes["max"].Value);
                quota.HDQuota = Convert.ToInt32(xd.DocumentElement["hd_quota"].InnerXml);
                quota.SDQuota = Convert.ToInt32(xd.DocumentElement["sd_quota"].InnerXml);
                return quota;
            }
            else
            {
                return null;
            }
        }

        public UploadTicket GetUploadTicket()
        {
            Validate();

            UploadTicket UT = new UploadTicket();
            XmlDocument xd = new XmlDocument();

            if (!CallMethod(videos_getUploadTicket, xd))
            {
                return null;
            }

            UT.Ticket = xd.DocumentElement.SelectSingleNode("ticket").Attributes["id"].Value;
            UT.URI = xd.DocumentElement.SelectSingleNode("ticket").Attributes["endpoint"].Value;

            SortedList parms = new SortedList();
            parms[Parameters.AUTHTOKEN] = _auth.Token;

            UT.URI = _utils.GetMethodURI(UT.URI, parms, _auth);

            return UT;
        }

        #endregion

        #region User

        public string GetApplicationLinkUrl()
        {
            XmlDocument xd = new XmlDocument();

            if (!CallMethod(auth_getFrob, xd))
            {
                return string.Empty;
            }
            string frob = xd.DocumentElement.SelectSingleNode("frob").InnerText;

            SortedList parms = new SortedList();
            parms[Parameters.FROB] = frob;
            parms[Parameters.PERMS] = "write";

            string loginURI = _utils.GetMethodURI(Vimeo_URL.URL_AUTH, parms, _auth);

            if (!string.IsNullOrEmpty(loginURI))
            {
                return loginURI;
            }
            else
            {
                return string.Empty;
            }
        }

        public string GetApplicationUserLinkToken(string frob)
        {
            XmlDocument xd = new XmlDocument();

            if (!CallMethod(auth_getToken, xd, "frob", frob))
            {
                return string.Empty;
            }

            string xmlToken = xd.DocumentElement.SelectSingleNode("auth/token").InnerXml;
            if (string.IsNullOrEmpty(xmlToken))
            {
                return string.Empty;
            }

            //NOTE: You can store the auth token into the database and use it multiple times.
            _auth.Token = xmlToken;

            return xmlToken;
        }

        public Dictionary<string, string> GetUserPresets()
        {
            if (!string.IsNullOrEmpty(_auth.Token))
            {
                Dictionary<string, string> userPresets = new Dictionary<string, string>();

                XmlDocument xd = new XmlDocument();
                if (CallMethod(videos_getPresets, xd))
                {
                    foreach (XmlNode n in xd.DocumentElement.SelectSingleNode("presets").ChildNodes)
                    {
                        userPresets.Add(n.Attributes["id"].Value, n.Attributes["name"].Value);
                    }

                    return userPresets;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                throw new Exception(TokenError);
            }
        }

        #endregion

        #region Common Methods

        private Boolean SendXMLManifest(string method, XmlDocument result, string xmlData, params string[] parms)
        {
            Validate();

            SortedList myParams = new SortedList();
            myParams[Parameters.METHOD] = method;

            if (!String.IsNullOrEmpty(_auth.Token))
            {
                myParams[Parameters.AUTHTOKEN] = _auth.Token;
            }
            for (int i = 0; i <= parms.Length - 1; i += 2)
            {
                myParams[parms[i]] = parms[i + 1];
            }

            string _URI = _utils.GetMethodURI(Vimeo_URL.URL_REST_VTWO, myParams, _auth);

            HttpWebRequest request = WebRequest.Create(_URI) as HttpWebRequest;

            // get a boundary string - used to separate parts of the form data
            string boundary = String.Format("----------{0:B}", Guid.NewGuid());

            // set the content type and method
            request.ContentType = String.Format("multipart/form-data; boundary={0}", boundary);
            request.Method = "POST";
            request.KeepAlive = true;
            request.Credentials = System.Net.CredentialCache.DefaultCredentials;

            // build the form part header
            StringBuilder header = new StringBuilder();
            header.AppendFormat("--{0}\r\n", boundary);
            header.Append("Content-Disposition: form-data; name=\"xml_manifest\"\r\n\r\n");

            // get the header as bytes
            byte[] headerBytes = Encoding.ASCII.GetBytes(header.ToString());

            // read the file
            byte[] fileBytes = Encoding.ASCII.GetBytes(xmlData);

            // get the footer bytes
            byte[] footerBytes = Encoding.ASCII.GetBytes(String.Format("\r\n--{0}--\r\n", boundary));

            // get the complete set of bytes
            byte[] data = headerBytes
            .Concat(fileBytes)
            .Concat(footerBytes).ToArray();

            // set the content length
            request.ContentLength = data.Length;

            // write the bytes to the request stream
            using (Stream s = request.GetRequestStream())
            {
                s.Write(data, 0, data.Length);
            }

            // get the response
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;

            // get the response stream and create the xdocument using the xmlreader
            using (Stream s = response.GetResponseStream())
            {
                result.Load(XmlReader.Create(s));
            }

            if (result.DocumentElement.GetAttribute("stat").Equals("ok"))
            {
                return true;
            }

            XmlNode xmlErr = result.DocumentElement.SelectSingleNode("err");
            if (xmlErr != null)
            {
                throw new Exception("Error calling method " + method + " - " + xmlErr.OuterXml.ToString());
            }
            else
            {
                throw new Exception("Error calling method " + method);
            }
        }

        private Boolean CallMethod(string method, XmlDocument result, params string[] parms)
        {
            SortedList myParams = new SortedList();
            myParams[Parameters.METHOD] = method;

            if (!String.IsNullOrEmpty(_auth.Token))
            {
                myParams[Parameters.AUTHTOKEN] = _auth.Token;
            }
            for (int i = 0; i <= parms.Length - 1; i += 2)
            {
                myParams[parms[i]] = parms[i + 1];
            }

            string _URI = _utils.GetMethodURI(Vimeo_URL.URL_REST_VTWO, myParams, _auth);
            HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(_URI);
            result.Load(wr.GetResponse().GetResponseStream());

            if (result.DocumentElement.GetAttribute("stat").Equals("ok"))
            {
                return true;
            }

            XmlNode xmlErr = result.DocumentElement.SelectSingleNode("err");
            if (xmlErr != null)
            {
                throw new Exception("Error calling method " + method + " - " + xmlErr.OuterXml.ToString());
            }
            else
            {
                throw new Exception("Error calling method " + method);
            }
        }

        /// <summary>
        /// Checks if the authentication object has a user token.
        /// </summary>
        private void Validate()
        {
            if (string.IsNullOrEmpty(_auth.Token))
            {
                throw new Exception(TokenError);
            }
        }

        #endregion

        #region Simplified Methods

        public string UploadSingleFile(string filePath)
        {
            UploadTicket ticket = GetUploadTicket();
            string fileHash = FileUpload(filePath, ticket);
            XmlDocument xmlDoc = _utils.GenerateUploadManifest(fileHash, null);
            return ConfirmUpload(ticket.Ticket, xmlDoc);
        }

        #endregion
    }
}