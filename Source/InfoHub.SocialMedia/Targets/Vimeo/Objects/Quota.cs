namespace InfoHub.SocialMedia.Targets.Vimeo.Objects
{
    public class Quota
    {
        public bool IsUserPlus { get; set; }
        public int Free { get; set; }
        public int Max { get; set; }
        public int HDQuota { get; set; }
        public int SDQuota { get; set; }

        public Quota(bool isUserPlus, int free, int max, int hDQuota, int sDQuota)
        {
            IsUserPlus = isUserPlus;
            Free = free;
            Max = max;
            HDQuota = hDQuota;
            SDQuota = sDQuota;
        }

        public Quota()
        {
        }
    }
}
