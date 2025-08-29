using RssCli.Interfaces;

namespace RssCli.Commands
{
    public class AddCommand : ICommand
    {
        private readonly IFeedStorage _storage;
        private readonly IFeedReader _feedReader;

        public string Name => "add";
        public string Description => "Add a new RSS feed";

        public AddCommand(IFeedStorage storage, IFeedReader feedReader)
        {
            _storage = storage;
            _feedReader = feedReader;
        }

        public async Task<int> ExecuteAsync(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Error: Missing URL argument");
                Console.WriteLine();
                ShowHelp();
                return 1;
            }

            // Check if user is asking for help
            if (args[1] == "--help" || args[1] == "-h")
            {
                ShowHelp();
                return 0;
            }

            var url = args[1];

            try
            {
                if (await _storage.FeedExistsAsync(url))
                {
                    Console.WriteLine($"Feed already exists: {url}");
                    return 0;
                }

                // Validate the feed
                Console.WriteLine($"Validating feed: {url}");
                var feed = await _feedReader.ValidateFeedAsync(url);
                Console.WriteLine($"Feed title: {feed.Title}");

                await _storage.AddFeedAsync(feed);
                Console.WriteLine($"Added: {url}");
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding feed: {ex.Message}");
                return 1;
            }
        }

        public void ShowHelp()
        {
            Console.WriteLine("USAGE:");
            Console.WriteLine("    RssCli add <url>");
            Console.WriteLine();
            Console.WriteLine("DESCRIPTION:");
            Console.WriteLine("    Add a new RSS feed URL to your collection");
            Console.WriteLine("    The feed will be validated before being added");
            Console.WriteLine();
            Console.WriteLine("EXAMPLES:");
            Console.WriteLine("    RssCli add https://feeds.bbci.co.uk/news/rss.xml");
            Console.WriteLine("    RssCli add https://feeds.feedburner.com/TechCrunch");
        }
    }
}
