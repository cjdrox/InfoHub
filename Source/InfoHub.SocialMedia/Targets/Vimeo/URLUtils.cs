namespace InfoHub.SocialMedia.Targets.Vimeo
{
    class Parameters
    {
        public const string METHOD = "method";
        public const string APIKEY = "api_key";
        public const string AUTHTOKEN = "auth_token";
        public const string TICKETID = "ticket_id";
        public const string PRIVACY = "privacy";
        public const string VIDEOID = "video_id";
        public const string FROB = "frob";
        public const string PERMS = "perms";
        public const string VTITLE = "title";
        public const string VDESCRIPTION = "description";
        public const string PRESETID = "preset_id";
        public const string UPLOADMANIFEST = "xml_manifest";
    }

    class Vimeo_URL
    {
        public const string URL_AUTH = "http://vimeo.com/services/auth/";
        public const string URL_REST = "http://vimeo.com/api/rest";
        public const string URL_REST_VTWO = "http://vimeo.com/api/rest/v2/";
    }
}
