using RssCli.Interfaces;
using System.Diagnostics;

namespace RssCli.Commands
{
    public class OpenCommand : ICommand
    {
        private readonly IFeedStorage _storage;
        private readonly IFeedReader _feedReader;

        public string Name => "open";

        public string Description => "Open post in browser by index";

        public OpenCommand(IFeedStorage storage, IFeedReader feedReader)
        {
            _storage = storage;
            _feedReader = feedReader;
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

                int index = 0;

                foreach (var feed in feeds)
                {
                    try
                    {
                        var items = await _feedReader.GetFeedItemsAsync(feed.Url, 5);

                        if (!items.Any())
                        {
                            Console.WriteLine("  No items found in this feed.");
                            continue;
                        }

                        foreach (var item in items)
                        {
                            index++;
                            if (Convert.ToInt32(args[1]) == index)
                            {
                                using (Process.Start(new ProcessStartInfo(item.Link) { UseShellExecute = true })) ;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"  Error reading feed: {ex.Message}");
                    }
                }

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error opening post: {ex.Message}");
                return 1;
            }
        }

        public void ShowHelp()
        {
            Console.WriteLine("USAGE:");
            Console.WriteLine("    RssCli open <index>");
            Console.WriteLine();
            Console.WriteLine("DESCRIPTION:");
            Console.WriteLine("    Opens the post in the browser");
            Console.WriteLine("    The post will be selected by the entered index");
            Console.WriteLine();
            Console.WriteLine("EXAMPLE:");
            Console.WriteLine("    RssCli open 6");
        }
    }
}