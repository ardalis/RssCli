using System.Text.Json;
using RssCli.Interfaces;
using RssCli.Models;

namespace RssCli.Services
{
    public class JsonFeedStorage : IFeedStorage
    {
        private readonly string _filePath;

        public JsonFeedStorage(string filePath = "feeds.json")
        {
            _filePath = filePath;
        }

        public async Task<List<RssFeed>> GetFeedsAsync()
        {
            if (!File.Exists(_filePath))
                return new List<RssFeed>();

            var json = await File.ReadAllTextAsync(_filePath);
            
            try
            {
                // Try to deserialize as new format first
                var feeds = JsonSerializer.Deserialize<List<RssFeed>>(json);
                return feeds ?? new List<RssFeed>();
            }
            catch (JsonException)
            {
                // Fall back to old format (list of strings)
                try
                {
                    var oldFeeds = JsonSerializer.Deserialize<List<string>>(json);
                    if (oldFeeds != null)
                    {
                        var convertedFeeds = oldFeeds.Select(url => new RssFeed 
                        { 
                            Url = url,
                            Title = null,
                            LastUpdated = null
                        }).ToList();
                        
                        // Save in new format
                        await SaveFeedsAsync(convertedFeeds);
                        return convertedFeeds;
                    }
                }
                catch (JsonException)
                {
                    // If both formats fail, return empty list
                }
                return new List<RssFeed>();
            }
        }

        public async Task SaveFeedsAsync(List<RssFeed> feeds)
        {
            var json = JsonSerializer.Serialize(feeds, new JsonSerializerOptions 
            { 
                WriteIndented = true 
            });
            await File.WriteAllTextAsync(_filePath, json);
        }

        public async Task AddFeedAsync(RssFeed feed)
        {
            var feeds = await GetFeedsAsync();
            if (!feeds.Any(f => f.Url == feed.Url))
            {
                feeds.Add(feed);
                await SaveFeedsAsync(feeds);
            }
        }

        public async Task<bool> FeedExistsAsync(string url)
        {
            var feeds = await GetFeedsAsync();
            return feeds.Any(f => f.Url == url);
        }
    }
}
