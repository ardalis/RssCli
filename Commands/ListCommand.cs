using RssCli.Interfaces;

namespace RssCli.Commands
{
    public class ListCommand : ICommand
    {
        private readonly IFeedStorage _storage;

        public string Name => "list";
        public string Description => "List all stored RSS feeds";

        public ListCommand(IFeedStorage storage)
        {
            _storage = storage;
        }

        public async Task<int> ExecuteAsync(string[] args)
        {
            // Check if user is asking for help
            if (args.Length > 1 && (args[1] == "--help" || args[1] == "-h"))
            {
                ShowHelp();
                return 0;
            }

            try
            {
                var feeds = await _storage.GetFeedsAsync();

                if (feeds.Count == 0)
                {
                    Console.WriteLine("No feeds added yet.");
                    return 0;
                }

                Console.WriteLine($"RSS Feeds ({feeds.Count}):");
                for (int i = 0; i < feeds.Count; i++)
                {
                    var feed = feeds[i];
                    var title = !string.IsNullOrEmpty(feed.Title) ? $" - {feed.Title}" : "";
                    Console.WriteLine($"  {i + 1}. {feed.Url}{title}");
                }
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error listing feeds: {ex.Message}");
                return 1;
            }
        }

        public void ShowHelp()
        {
            Console.WriteLine("USAGE:");
            Console.WriteLine("    RssCli list");
            Console.WriteLine();
            Console.WriteLine("DESCRIPTION:");
            Console.WriteLine("    Display all stored RSS feed URLs in a numbered list");
            Console.WriteLine();
            Console.WriteLine("EXAMPLE:");
            Console.WriteLine("    RssCli list");
        }
    }
}
