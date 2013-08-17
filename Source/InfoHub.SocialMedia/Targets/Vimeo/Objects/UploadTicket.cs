namespace InfoHub.SocialMedia.Targets.Vimeo.Objects
{
    public class UploadTicket
    {
        public string Ticket { get; set; }
        public string URI { get; set; }

        public UploadTicket(string ticket, string uRI)
        {
            URI = uRI;
            Ticket = ticket;
        }

        public UploadTicket()
        {
        }
    }
}
