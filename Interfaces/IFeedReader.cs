using RssCli.Models;

namespace RssCli.Interfaces
{
    public interface IFeedReader
    {
        Task<RssFeed> ValidateFeedAsync(string url);
        Task<List<RssItem>> GetFeedItemsAsync(string url, int maxItems = 5);
    }
}
