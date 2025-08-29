using RssCli.Commands;
using RssCli.Interfaces;
using RssCli.Services;

namespace RssCli
{
    public class CommandLineApp
    {
        private readonly Dictionary<string, ICommand> _commands;
        private readonly IHelpProvider _helpProvider;

        public CommandLineApp()
        {
            // Dependency injection setup
            var storage = new JsonFeedStorage();
            var feedReader = new FeedReaderService();

            // Initialize commands
            _commands = new Dictionary<string, ICommand>
            {
                ["add"] = new AddCommand(storage, feedReader),
                ["list"] = new ListCommand(storage),
                ["show"] = new ShowCommand(storage, feedReader)
            };

            _helpProvider = new HelpProvider(_commands);
        }

        public async Task<int> RunAsync(string[] args)
        {
            if (args.Length == 0)
            {
                _helpProvider.ShowMainHelp();
                return 0;
            }

            var command = args[0].ToLower();

            // Check for help flags
            if (command == "--help" || command == "-h" || command == "help")
            {
                _helpProvider.ShowMainHelp();
                return 0;
            }

            // Execute command
            if (_commands.TryGetValue(command, out var cmd))
            {
                return await cmd.ExecuteAsync(args);
            }

            // Unknown command
            Console.WriteLine($"Unknown command: {command}");
            Console.WriteLine("Use 'RssCli --help' or 'RssCli -h' to see available commands.");
            return 1;
        }
    }
}
