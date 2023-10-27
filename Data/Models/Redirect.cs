namespace UrlSh.Data.Models
{
    public class Redirect : Base
    {
        public string? Code { get; set; }
        public Uri Url { get; set; } = null!;
        public ICollection<RedirectLog> Logs { get; set; } = new HashSet<RedirectLog>();
    }
}
