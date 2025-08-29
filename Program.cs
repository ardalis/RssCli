using System.Text.Json;
using CodeHollow.FeedReader;

var feedsFile = "feeds.json";

if (args.Length == 0)
{
    ShowHelp();
    return 0;
}

var command = args[0].ToLower();

// Check for help flags
if (command == "--help" || command == "-h" || command == "help")
{
    ShowHelp();
    return 0;
}

switch (command)
{
    case "add":
        if (args.Length < 2)
        {
            Console.WriteLine("Error: Missing URL argument");
            Console.WriteLine();
            Console.WriteLine("USAGE:");
            Console.WriteLine("    RssCli add <url>");
            Console.WriteLine();
            Console.WriteLine("DESCRIPTION:");
            Console.WriteLine("    Add a new RSS feed URL to your collection");
            Console.WriteLine();
            Console.WriteLine("EXAMPLES:");
            Console.WriteLine("    RssCli add https://feeds.bbci.co.uk/news/rss.xml");
            Console.WriteLine("    RssCli add https://feeds.feedburner.com/TechCrunch");
            return 1;
        }
        
        // Check if user is asking for help on this command
        if (args[1] == "--help" || args[1] == "-h")
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
            return 0;
        }
        
        return await AddFeed(feedsFile, args[1]);

    case "list":
        // Check if user is asking for help on this command
        if (args.Length > 1 && (args[1] == "--help" || args[1] == "-h"))
        {
            Console.WriteLine("USAGE:");
            Console.WriteLine("    RssCli list");
            Console.WriteLine();
            Console.WriteLine("DESCRIPTION:");
            Console.WriteLine("    Display all stored RSS feed URLs in a numbered list");
            Console.WriteLine();
            Console.WriteLine("EXAMPLE:");
            Console.WriteLine("    RssCli list");
            return 0;
        }
        return await ListFeeds(feedsFile);

    case "show":
        // Check if user is asking for help on this command
        if (args.Length > 1 && (args[1] == "--help" || args[1] == "-h"))
        {
            Console.WriteLine("USAGE:");
            Console.WriteLine("    RssCli show");
            Console.WriteLine();
            Console.WriteLine("DESCRIPTION:");
            Console.WriteLine("    Display the latest 5 posts from each stored RSS feed");
            Console.WriteLine("    Shows post title, publication date, and link");
            Console.WriteLine();
            Console.WriteLine("EXAMPLE:");
            Console.WriteLine("    RssCli show");
            return 0;
        }
        return await ShowPosts(feedsFile);

    default:
        Console.WriteLine($"Unknown command: {command}");
        Console.WriteLine("Use 'RssCli --help' or 'RssCli -h' to see available commands.");
        return 1;
}

static void ShowHelp()
{
    Console.WriteLine("RSS CLI - RSS Feed Manager");
    Console.WriteLine();
    Console.WriteLine("USAGE:");
    Console.WriteLine("    RssCli [OPTIONS] [COMMAND]");
    Console.WriteLine();
    Console.WriteLine("OPTIONS:");
    Console.WriteLine("    -h, --help       Show this help message");
    Console.WriteLine();
    Console.WriteLine("COMMANDS:");
    Console.WriteLine("    add <url>        Add a new RSS feed");
    Console.WriteLine("                     Example: RssCli add https://feeds.bbci.co.uk/news/rss.xml");
    Console.WriteLine();
    Console.WriteLine("    list             List all stored RSS feeds");
    Console.WriteLine("                     Shows numbered list of all feed URLs");
    Console.WriteLine();
    Console.WriteLine("    show             Show recent posts from all feeds");
    Console.WriteLine("                     Displays latest 5 posts from each stored feed");
    Console.WriteLine();
    Console.WriteLine("EXAMPLES:");
    Console.WriteLine("    RssCli --help");
    Console.WriteLine("    RssCli add https://feeds.bbci.co.uk/news/rss.xml");
    Console.WriteLine("    RssCli add https://feeds.feedburner.com/TechCrunch");
    Console.WriteLine("    RssCli list");
    Console.WriteLine("    RssCli show");
    Console.WriteLine();
    Console.WriteLine("DATA STORAGE:");
    Console.WriteLine("    Feed URLs are stored in 'feeds.json' in the current directory");
}

static async Task<int> AddFeed(string feedsFile, string url)
{
    try
    {
        List<string> feeds = new();
        if (File.Exists(feedsFile))
        {
            var json = await File.ReadAllTextAsync(feedsFile);
            feeds = JsonSerializer.Deserialize<List<string>>(json) ?? new();
        }

        if (feeds.Contains(url))
        {
            Console.WriteLine($"Feed already exists: {url}");
            return 0;
        }

        // Validate the feed by trying to read it
        Console.WriteLine($"Validating feed: {url}");
        var feed = await FeedReader.ReadAsync(url);
        Console.WriteLine($"Feed title: {feed.Title}");

        feeds.Add(url);
        await File.WriteAllTextAsync(feedsFile, JsonSerializer.Serialize(feeds, new JsonSerializerOptions { WriteIndented = true }));
        Console.WriteLine($"Added: {url}");
        return 0;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error adding feed: {ex.Message}");
        return 1;
    }
}

static async Task<int> ListFeeds(string feedsFile)
{
    try
    {
        if (!File.Exists(feedsFile))
        {
            Console.WriteLine("No feeds added yet.");
            return 0;
        }

        var json = await File.ReadAllTextAsync(feedsFile);
        var feeds = JsonSerializer.Deserialize<List<string>>(json) ?? new();

        if (feeds.Count == 0)
        {
            Console.WriteLine("No feeds added yet.");
            return 0;
        }

        Console.WriteLine($"RSS Feeds ({feeds.Count}):");
        for (int i = 0; i < feeds.Count; i++)
        {
            Console.WriteLine($"  {i + 1}. {feeds[i]}");
        }
        return 0;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error listing feeds: {ex.Message}");
        return 1;
    }
}

static async Task<int> ShowPosts(string feedsFile)
{
    try
    {
        if (!File.Exists(feedsFile))
        {
            Console.WriteLine("No feeds added yet.");
            return 0;
        }

        var json = await File.ReadAllTextAsync(feedsFile);
        var feeds = JsonSerializer.Deserialize<List<string>>(json) ?? new();

        if (feeds.Count == 0)
        {
            Console.WriteLine("No feeds added yet.");
            return 0;
        }

        foreach (var url in feeds)
        {
            Console.WriteLine($"\n=== {url} ===");
            try
            {
                var feed = await FeedReader.ReadAsync(url);
                var items = feed.Items.Take(5);

                if (!items.Any())
                {
                    Console.WriteLine("  No items found in this feed.");
                    continue;
                }

                foreach (var item in items)
                {
                    var title = item.Title ?? "No title";
                    var link = item.Link ?? "No link";
                    var publishDate = item.PublishingDate?.ToString("yyyy-MM-dd") ?? "Unknown date";
                    Console.WriteLine($"  [{publishDate}] {title}");
                    Console.WriteLine($"    {link}");
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
        Console.WriteLine($"Error showing posts: {ex.Message}");
        return 1;
    }
}
