namespace RssCli.Models
{
    public class RssItem
    {
        public string Title { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;
        public DateTime? PublishDate { get; set; }
        public string? Description { get; set; }
    }
}
