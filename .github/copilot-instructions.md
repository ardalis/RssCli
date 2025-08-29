# RSS CLI - AI Coding Agent Instructions

## Architecture Overview

This is a **SOLID-compliant Command Line Interface** for RSS feed management built with .NET 8.0. The codebase follows a **clean architecture** pattern with clear separation of concerns and dependency injection.

### Key Architectural Components

- **Commands/** - Individual command implementations (`AddCommand`, `ListCommand`, `ShowCommand`)
- **Services/** - Business logic (`JsonFeedStorage`, `FeedReaderService`, `HelpProvider`)
- **Interfaces/** - Service contracts (`ICommand`, `IFeedStorage`, `IFeedReader`, `IHelpProvider`)
- **Models/** - Data models (`RssFeed`, `RssItem`)
- **CommandLineApp.cs** - Main application orchestrator with manual DI container

## Command Pattern Implementation

Each command follows the `ICommand` interface with consistent patterns:

```csharp
public class AddCommand : ICommand
{
    public string Name => "add";
    public string Description => "Add a new RSS feed";
    
    public async Task<int> ExecuteAsync(string[] args) { }
    public void ShowHelp() { }
}
```

### Adding New Commands
1. Create class in `Commands/` implementing `ICommand`
2. Inject required services via constructor
3. Register in `CommandLineApp` constructor dictionary
4. Follow existing help patterns with `--help`/`-h` support

## Dependency Injection Pattern

**Manual DI container** in `CommandLineApp` constructor - no external DI framework used:

```csharp
var storage = new JsonFeedStorage();
var feedReader = new FeedReaderService();
_commands = new Dictionary<string, ICommand>
{
    ["add"] = new AddCommand(storage, feedReader),
    ["list"] = new ListCommand(storage),
    ["show"] = new ShowCommand(storage, feedReader)
};
```

## Data Storage Strategy

**JSON file-based storage** with backward compatibility migration in `JsonFeedStorage.GetFeedsAsync()`:
- Detects old format (string array) vs new format (RssFeed objects)
- Auto-migrates old format to new format on first read
- Thread-safe file operations with proper error handling

## Testing Commands

```powershell
# Build and run
dotnet build
dotnet run -- --help
dotnet run -- add https://example.com/feed.rss
dotnet run -- list
dotnet run -- show

# Test specific command help
dotnet run -- add --help
dotnet run -- list --help
dotnet run -- show --help
```

## Error Handling Conventions

- **Return codes**: 0 for success, 1 for errors
- **Exception wrapping**: Services wrap external exceptions with descriptive messages
- **Graceful degradation**: Individual feed failures don't stop processing other feeds
- **User-friendly messages**: Technical exceptions converted to actionable user guidance

## Key Dependencies

- **CodeHollow.FeedReader** - RSS/Atom feed parsing (wrapped in `FeedReaderService`)
- **System.Text.Json** - JSON serialization with `WriteIndented = true` for readability

## Project Conventions

- **Async/await** throughout for I/O operations
- **Nullable reference types** enabled
- **Interface segregation** - focused, single-responsibility interfaces
- **Constructor dependency injection** pattern consistently applied
- **Descriptive help text** with examples for all commands and main app
- **One type per file** - Each interface and model in its own file for better organization

## File Structure

```
RssCli/
├── Program.cs                 # Entry point (4 lines)
├── CommandLineApp.cs          # Main orchestrator with DI
├── Commands/
│   ├── AddCommand.cs         # Add RSS feed command
│   ├── ListCommand.cs        # List all feeds command
│   └── ShowCommand.cs        # Show recent posts command
├── Services/
│   ├── JsonFeedStorage.cs    # JSON file storage with migration
│   ├── FeedReaderService.cs  # RSS feed parsing wrapper
│   └── HelpProvider.cs       # Help text generation
├── Interfaces/
│   ├── ICommand.cs           # Command contract
│   ├── IFeedStorage.cs       # Storage service contract
│   ├── IFeedReader.cs        # Feed reading service contract
│   └── IHelpProvider.cs      # Help service contract
├── Models/
│   ├── RssFeed.cs            # RSS feed data model
│   └── RssItem.cs            # RSS item data model
└── feeds.json                # Data storage file
```

## Development Workflow

1. **Testing Changes**: Always run `dotnet build` first, then test commands
2. **Help Documentation**: Every command must support `--help` and `-h` flags
3. **Error Handling**: Return 1 for errors, 0 for success; wrap exceptions with user-friendly messages
4. **Data Migration**: The `JsonFeedStorage` handles backward compatibility automatically
