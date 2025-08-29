using CodeHollow.FeedReader;
using RssCli.Interfaces;
using RssCli.Models;

namespace RssCli.Services
{
    public class FeedReaderService : IFeedReader
    {
        public async Task<RssFeed> ValidateFeedAsync(string url)
        {
            try
            {
                var feed = await CodeHollow.FeedReader.FeedReader.ReadAsync(url);
                return new RssFeed
                {
                    Url = url,
                    Title = feed.Title,
                    LastUpdated = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Invalid RSS feed: {ex.Message}", ex);
            }
        }

        public async Task<List<RssItem>> GetFeedItemsAsync(string url, int maxItems = 5)
        {
            try
            {
                var feed = await CodeHollow.FeedReader.FeedReader.ReadAsync(url);
                return feed.Items
                    .Take(maxItems)
                    .Select(item => new RssItem
                    {
                        Title = item.Title ?? "No title",
                        Link = item.Link ?? "No link",
                        PublishDate = item.PublishingDate,
                        Description = item.Description
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error reading feed: {ex.Message}", ex);
            }
        }
    }
}
