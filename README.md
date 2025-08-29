# RSS CLI

A simple command-line tool for managing RSS feeds and viewing recent posts.

## Features

- Add RSS feed URLs to your personal collection
- List all stored RSS feeds
- Display recent posts from all your feeds
- Automatic feed validation when adding new feeds
- JSON-based local storage

## Installation

1. Clone or download this repository
2. Make sure you have .NET 8.0 or later installed
3. Build the project:
   ```bash
   dotnet build -c Release
   ```

## Usage

### Show Help
```bash
dotnet run -- --help
dotnet run -- -h
dotnet run -- help
```

You can also get help for individual commands:
```bash
dotnet run -- add --help
dotnet run -- list -h
dotnet run -- show --help
```

### Add a New RSS Feed
```bash
dotnet run -- add <feed-url>
```

Example:
```bash
dotnet run -- add https://feeds.bbci.co.uk/news/rss.xml
```

### List All Feeds
```bash
dotnet run -- list
```

### Show Recent Posts from All Feeds
```bash
dotnet run -- show
```

## Examples

```bash
# Show help
dotnet run -- --help

# Add some feeds
dotnet run -- add https://feeds.bbci.co.uk/news/rss.xml
dotnet run -- add https://feeds.feedburner.com/TechCrunch

# Get help for specific commands
dotnet run -- add --help

# List your feeds
dotnet run -- list

# See recent posts
dotnet run -- show
```

## Data Storage

The CLI stores feed URLs in a `feeds.json` file in the same directory as the executable. This file is created automatically when you add your first feed.

## Dependencies

- [CodeHollow.FeedReader](https://github.com/codehollow/FeedReader) - For parsing RSS/Atom feeds
- .NET 8.0 - Target framework

## Building a Standalone Executable

To create a self-contained executable:

```bash
# Windows
dotnet publish -c Release -r win-x64 --self-contained

# macOS
dotnet publish -c Release -r osx-x64 --self-contained

# Linux
dotnet publish -c Release -r linux-x64 --self-contained
```

The executable will be in the `bin/Release/net8.0/<runtime>/publish/` directory.
