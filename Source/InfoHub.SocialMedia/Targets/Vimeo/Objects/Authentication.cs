namespace InfoHub.SocialMedia.Targets.Vimeo.Objects
{
    public class Authentication
    {
        public string PublicKey { get; set; }
        public string SecretKey { get; set; }
        public string Token { get; set; }

        //This will cause the application to request a token
        public Authentication(string publicKey, string secretKey)
        {
            PublicKey = publicKey;
            SecretKey = secretKey;
            Token = string.Empty;
        }

        //This will cause the application to request a token
        public Authentication(string publicKey, string secretKey, string token)
        {
            PublicKey = publicKey;
            SecretKey = secretKey;
            Token = token;
        }
    }
}
