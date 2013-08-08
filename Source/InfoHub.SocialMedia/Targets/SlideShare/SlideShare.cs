using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace InfoHub.FaceBook.Targets.SlideShare
{
    public class SlideShare
    {
        private readonly string _apiKey;
        private readonly string _sharedSecret;
        private readonly string _timestamp;

        /// <summary>
        /// Pass your API key and Shared Secret you received on e-mail from Slideshare.
        /// If you don't have these keys, you cannot use the API.
        /// You need to apply for an API key on slideshare.net for getting one. Do not share your keys with anyone.
        /// </summary>
        /// <param name="apiKey">Your API key</param>
        /// <param name="sharedSecret">Your Shared Secret</param>
        public SlideShare(string apiKey, string sharedSecret)
        {
            _apiKey = apiKey;
            _sharedSecret = sharedSecret;
            _timestamp = Convert.ToInt32(Math.Floor((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds)).ToString(CultureInfo.InvariantCulture);
        }

        #region Slideshows
        /// <summary>
        /// Get slideshow
        /// </summary>
        /// <param name="slideshowId">ID of the slideshow</param>
        public string GetSlideshow(int slideshowId)
        {
            var parameters = GetParameterBase();
            parameters.Add("slideshow_id", slideshowId);

            return GetCommand.Execute("http://www.slideshare.net/api/2/get_slideshow", parameters);
        }

        /// <summary>
        /// Get slideshow
        /// </summary>
        /// <param name="slideshowUrl">URL of the slideshow</param>
        public string GetSlideshow(string slideshowUrl)
        {
            var parameters = GetParameterBase();
            parameters.Add("slideshow_url", slideshowUrl);

            return GetCommand.Execute("http://www.slideshare.net/api/2/get_slideshow", parameters);
        }

        /// <summary>
        /// Gets all public slideshows tagged with a particular tag on Slideshare.
        /// Ex. To see all slideshows tagged as "web", see http://www.slideshare.net/tag/web
        /// </summary>
        /// <param name="tags">The tag to search</param>
        /// <param name="offset">OPTIONAL: Number of slideshows to skip starting from the beginning</param>
        /// <param name="limit">OPTIONAL: Restrict response to these many slideshows</param>
        public string GetSlideshowsForTag(string tags, int offset, int limit)
        {
            var parameters = GetParameterBase();
            parameters.Add("tag", tags);

            if (offset != 0)
            {
                parameters.Add("offset", offset);
            }

            if (limit != 0)
            {
                parameters.Add("limit", limit);
            }

            return GetCommand.Execute("http://www.slideshare.net/api/2/get_slideshows_by_tag", parameters);
        }

        /// <summary>
        /// Gets all public slideshows posted in a particular group on Slideshare.
        /// To see the groups section, see http://www.slideshare.net/groups
        /// </summary>
        /// <param name="groupname">Name of your group. Ex. If your group URL is http://www.slideshare.net/group/web-wednesday, then group name is web-wednesday, not Web Wednesday</param>
        /// <param name="offset">OPTIONAL: Number of slideshows to skip starting from the beginning</param>
        /// <param name="limit">OPTIONAL: Restrict response to these many slideshows</param>
        public string GetSlideshowsForGroup(string groupname, int offset, int limit)
        {
            var parameters = GetParameterBase();
            parameters.Add("group_name", groupname);

            if (offset != 0)
            {
                parameters.Add("offset", offset);
            }

            if (limit != 0)
            {
                parameters.Add("limit", limit);
            }

            return GetCommand.Execute("http://www.slideshare.net/api/2/get_slideshows_by_group", parameters);
        }
        
        /// <summary>
        /// Gets all public slideshows posted for a particular user on Slideshare.
        /// To see a sample user's slideshow collection, see http://www.slideshare.net/jboutelle/slideshows
        /// </summary>
        /// <param name="username">Name of the slideshare user. Ex. If your slidespace URL is http://www.slideshare.net/gauravgupta, then group name is gauravgupta, not Gaurav Gupta</param>
        /// <param name="offset">OPTIONAL: Number of slideshows to skip starting from the beginning</param>
        /// <param name="limit">OPTIONAL: Restrict response to these many slideshows</param>
        public string GetSlideshowsForUser(string username, int offset, int limit)
        {
            var parameters = GetParameterBase();
            parameters.Add("username_for", username);

            if (offset != 0)
            {
                parameters.Add("offset", offset);
            }

            if (limit != 0)
            {
                parameters.Add("limit", limit);
            }

            return GetCommand.Execute("http://www.slideshare.net/api/2/get_slideshows_by_user", parameters);
        }

        /// <summary>
        /// Search for slideshows
        /// </summary>
        /// <param name="query">Query string to use in search</param>
        public string SlideshowSearch(string query)
        {
            var parameters = GetParameterBase();
            parameters.Add("q", query);

            return GetCommand.Execute("http://www.slideshare.net/api/2/search_slideshows", parameters);
        }

        /// <summary>
        /// Search for slideshows
        /// </summary>
        /// <param name="query">Query string to use in search</param>
        /// <param name="page">OPTIONAL: The page number of the results (works in conjunction with itemsPerPage), default is 1</param>
        /// <param name="itemsPerPage">OPTIONAL: Number of results to return per page, default is 12</param>
        /// <param name="language">OPTIONAL: Language of slideshows (default is English, 'en')('**':All,'es':Spanish,'pt':Portuguese,'fr':French,'it':Italian,'nl':Dutch,'de':German,'zh':Chinese,'ja':Japanese,'ko':Korean,'ro':Romanian,'!!':Other) </param>
        /// <param name="sortOrder">OPTIONAL: Sort order (default is 'relevance')</param>
        /// <returns></returns>
        public string SlideshowSearch(string query, int page, int itemsPerPage, string language, SortOrder sortOrder)
        {
            var parameters = GetParameterBase();
            parameters.Add("q", query);

            if (page != 0)
            {
                parameters.Add("page", page);
            }

            if (itemsPerPage != 0)
            {
                parameters.Add("items_per_page", itemsPerPage);
            }

            if (!string.IsNullOrEmpty(language))
            {
                parameters.Add("lang", language);
            }

            parameters.Add("sort", sortOrder);

            return GetCommand.Execute("http://www.slideshare.net/api/2/search_slideshows", parameters);
        }
        #endregion
        #region Users
        /// <summary>
        /// Get all groups for a particular user
        /// </summary>
        /// <param name="usernameFor">Username of user whose groups are being requested</param>
        /// <param name="username">OPTIONAL: username of the requesting user</param>
        /// <param name="password">OPTIONAL: password of the requesting user</param>
        /// <returns></returns>
        public string GetUserGroups(string usernameFor, string username, string password)
        {
            var parameters = this.GetParameterBase();
            parameters.Add("username_for", usernameFor);

            if (!string.IsNullOrEmpty(username))
            {
                parameters.Add("username", username);
            }

            if (!string.IsNullOrEmpty(password))
            {
                parameters.Add("password", password);
            }

            return GetCommand.Execute("http://www.slideshare.net/api/2/get_user_groups", parameters);
        }

        /// <summary>
        /// Get all contacts of a particular user
        /// </summary>
        /// <param name="usernameFor">Username of user whose contacts are being requested</param>
        /// <returns></returns>
        public string GetUserContacts(string usernameFor)
        {
            var parameters = this.GetParameterBase();
            parameters.Add("username_for", usernameFor);

            return GetCommand.Execute("http://www.slideshare.net/api/2/get_user_contacts", parameters);
        }

        /// <summary>
        /// Get all user tags
        /// </summary>
        /// <param name="username">Username of the requesting user</param>
        /// <param name="password">Password of the requesting user</param>
        /// <returns></returns>
        public string GetUserTags(string username, string password)
        {
            var parameters = this.GetParameterBase();
            parameters.Add("username", username);
            parameters.Add("password", password);

            return GetCommand.Execute("http://www.slideshare.net/api/2/get_user_tags", parameters);
        }
        #endregion

        #region Upload/Edit/Delete
        /// <summary>
        /// Edit Existing slideshow
        /// </summary>
        /// <param name="username">Username of the slideshow owner</param>
        /// <param name="password">Password of the slideshow owner</param>
        /// <param name="slideshowId">Slideshow ID</param>
        /// <param name="slideshowTitle">OPTIONAL: Title of the slideshow</param>
        /// <param name="slideshowDescription">OPTIONAL: Description of the slideshow</param>
        /// <param name="slideshowTags">OPTIONAL: Tags of the slideshow</param>
        /// <param name="makeSlideshowPrivate">OPTIONAL: Make slideshow private</param>
        /// <param name="generateSecretUrl">OPTIONAL: Generate a secret URL for the slideshow. Requires makeSlideshowPrivate to be true</param>
        /// <param name="allowEmbeds">OPTIONAL: Sets if other websites should be allowed to embed the slideshow. Requires makeSlideshowPrivate to be true</param>
        /// <param name="shareWithContacts">OPTIONAL: Sets if your contacts on SlideShare can view the slideshow. Requires makeSlideshowPrivate to be true</param>
        public string EditSlideshow(string username, string password, int slideshowId, string slideshowTitle, string slideshowDescription, string slideshowTags, bool makeSlideshowPrivate, bool generateSecretUrl, bool allowEmbeds, bool shareWithContacts)
        {
            var parameters = this.GetParameterBase();
            parameters.Add("username", username);
            parameters.Add("password", password);
            parameters.Add("slideshow_id", slideshowId);

            if (!string.IsNullOrEmpty(slideshowTitle))
            {
                parameters.Add("slideshow_title", slideshowTitle);
            }

            if (!string.IsNullOrEmpty(slideshowDescription))
            {
                parameters.Add("slideshow_description", slideshowDescription);
            }

            if (!string.IsNullOrEmpty(slideshowTags))
            {
                parameters.Add("slideshow_tags", slideshowTags);
            }

            parameters.Add("make_slideshow_private", Helper.GetYorN(makeSlideshowPrivate));
            
            if (makeSlideshowPrivate)
            {
                parameters.Add("generate_secret_url", Helper.GetYorN(generateSecretUrl));
                parameters.Add("allow_embeds", Helper.GetYorN(allowEmbeds));
                parameters.Add("share_with_contacts", Helper.GetYorN(shareWithContacts));
            }

            return PostCommand.Execute("http://www.slideshare.net/api/2/edit_slideshow", parameters);
        }

        /// <summary>
        /// Delete slideshow
        /// </summary>
        /// <param name="username">Username of the slideshow owner</param>
        /// <param name="password">Password of the slideshow owner</param>
        /// <param name="slideshowId">Slideshow ID</param>
        public string DeleteSlideshow(string username, string password, int slideshowId)
        {
            var parameters = this.GetParameterBase();
            parameters.Add("username", username);
            parameters.Add("password", password);
            parameters.Add("slideshow_id", slideshowId.ToString(CultureInfo.InvariantCulture));

            return PostCommand.Execute("http://www.slideshare.net/api/2/delete_slideshow", parameters);
        }

        /// <summary>
        /// Upload a new slideshow
        /// </summary>
        /// <param name="username">Username of the slideshow owner</param>
        /// <param name="password">Password of the slideshow owner</param>
        /// <param name="slideshowTitle">Slideshow's title</param>
        /// <param name="slideshowFilePath">Path to presentation file</param>
        /// <param name="slideshowFilename">Slideshow filename</param>
        /// <param name="slideshowDescription">OPTIONAL: Description of the slideshow</param>
        /// <param name="slideshowTags">OPTIONAL: Tags of the slideshow</param>
        /// <param name="makeSrcPublic">OPTIONAL: True if you want users to be able to download the ppt file, False otherwise. Default is True</param>
        /// <param name="makeSlideshowPrivate">OPTIONAL: Make slideshow private</param>
        /// <param name="generateSecretUrl">OPTIONAL: Generate a secret URL for the slideshow. Requires makeSlideshowPrivate to be true</param>
        /// <param name="allowEmbeds">OPTIONAL: Sets if other websites should be allowed to embed the slideshow. Requires makeSlideshowPrivate to be true</param>
        /// <param name="shareWithContacts">OPTIONAL: Sets if your contacts on SlideShare can view the slideshow. Requires makeSlideshowPrivate to be true</param>
        /// <returns></returns>
        public string UploadSlideshow(string username, string password, string slideshowTitle, string slideshowFilePath, string slideshowFilename, string slideshowDescription, string slideshowTags, bool makeSrcPublic, bool makeSlideshowPrivate, bool generateSecretUrl, bool allowEmbeds, bool shareWithContacts)
        {
            var fs = new FileStream(slideshowFilePath, FileMode.Open, FileAccess.Read);
            var data = new byte[fs.Length];
            fs.Read(data, 0, data.Length);
            fs.Close();

            var parameters = this.GetParameterBase();
            parameters.Add("username", username);
            parameters.Add("password", password);
            parameters.Add("slideshow_title", slideshowTitle);
            parameters.Add("slideshow_srcfile", new PostCommand.FileParameter(data, slideshowFilename, "application/ms-powerpoint"));
            if (!string.IsNullOrEmpty(slideshowDescription))
            {
                parameters.Add("slideshow_description", slideshowDescription);
            }

            if (!string.IsNullOrEmpty(slideshowTags))
            {
                parameters.Add("slideshow_tags", slideshowTags);
            }

            parameters.Add("make_src_public", Helper.GetYorN(makeSrcPublic));
            parameters.Add("make_slideshow_private", Helper.GetYorN(makeSlideshowPrivate));
            if (makeSlideshowPrivate)
            {
                parameters.Add("generate_secret_url", Helper.GetYorN(generateSecretUrl));
                parameters.Add("allow_embeds", Helper.GetYorN(allowEmbeds));
                parameters.Add("share_with_contacts", Helper.GetYorN(shareWithContacts));
            }

            return PostCommand.Execute("http://www.slideshare.net/api/2/upload_slideshow", parameters);
        }
        #endregion

        private Dictionary<string, object> GetParameterBase()
        {
            var parameterBase = new Dictionary<string, object>
                                    {
                                        {"api_key", _apiKey},
                                        {"ts", _timestamp},
                                        {"hash", Helper.CalculateSHA1(_sharedSecret + _timestamp, Encoding.UTF8)}
                                    };

            return parameterBase;
        }
    }
}