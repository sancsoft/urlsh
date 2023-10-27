namespace UrlSh.Data.Models
{
    public class RedirectLog : Base
    {
        public int RedirectId { get; set; }
        public Redirect Redirect { get; set; } = null!;
        public string? IPAddress { get; set; }
        public string? UserAgent { get; set; }
        public string? Referer { get; set; }
    }
}
