using RssCli.Interfaces;

namespace RssCli.Services
{
    public class HelpProvider : IHelpProvider
    {
        private readonly Dictionary<string, ICommand> _commands;

        public HelpProvider(Dictionary<string, ICommand> commands)
        {
            _commands = commands;
        }

        public void ShowMainHelp()
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
            
            foreach (var command in _commands.Values)
            {
                Console.WriteLine($"    {command.Name,-15}  {command.Description}");
            }
            
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

        public void ShowCommandHelp(string commandName)
        {
            if (_commands.TryGetValue(commandName, out var command))
            {
                command.ShowHelp();
            }
            else
            {
                Console.WriteLine($"Unknown command: {commandName}");
                Console.WriteLine("Use 'RssCli --help' to see available commands.");
            }
        }
    }
}
