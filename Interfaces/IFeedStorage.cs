using RssCli.Models;

namespace RssCli.Interfaces
{
    public interface IFeedStorage
    {
        Task<List<RssFeed>> GetFeedsAsync();
        Task SaveFeedsAsync(List<RssFeed> feeds);
        Task AddFeedAsync(RssFeed feed);
        Task<bool> FeedExistsAsync(string url);
    }
}
