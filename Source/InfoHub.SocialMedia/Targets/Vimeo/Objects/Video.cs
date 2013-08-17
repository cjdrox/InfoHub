namespace InfoHub.SocialMedia.Targets.Vimeo.Objects
{
    public class Video
    {
        public string VideoId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string UploadDate { get; set; }
        public string Duration { get; set; }
        public string[] Thumbnails { get; set; }

        public Video(string videoId, string title, string description, string uploadDate, string duration, string[] thumbnails)
        {
            VideoId = videoId;
            Title = title;
            Description = description;
            UploadDate = uploadDate;
            Duration = duration;
            Thumbnails = thumbnails;
        }

        public Video()
        {
        }
    }
}
