namespace RssCli.Models
{
    public class RssFeed
    {
        public string Url { get; set; } = string.Empty;
        public string? Title { get; set; }
        public DateTime? LastUpdated { get; set; }
    }
}
