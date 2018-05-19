# What is this?

The "osu-wiki Translation Tracking Tools", or osuWikiTTT for short, exist to help translators of the wiki keep track of:

- What articles exist in english
- What articles still need to be translated to language X

## What can it do?

This program can check the folder structure of the osu-wiki repository (currently only local) and parse it into a tree of articles (_soon: and the languages that already exist for each article_).

It can then further use this "tree" to create text output, for example it can create a kind of "tracking list" using [GitHub-flavored Markdown](https://guides.github.com/features/mastering-markdown/#GitHub-flavored-markdown) that roughly looks like this:

```markdown
- [ ] Article 1
- [ ] Article 2
    - [ ] Article 2.1
    - [ ] Article 2.2
    - [ ] Article 2.3
- [ ] Article 3
    - [ ] Article 3.1
```

and so on (_soon: it will be able to check all boxes for your translation, provided that you enter your language code._).

## How do I use it?

**Note: This program is currently configured to do exactly one thing: Find all articles in the specified directory and supply a text using GFM that shows all of them in an organized fashion.**

The exact usage depends on the platform and type of framework used. Though .NET Core will work on all platforms, using .NET Framework is easier on Windows since you do not have to install the [.NET Core Runtime](https://www.microsoft.com/net/download/Windows/run).

This tool will only take very simple input parameters:

- `wikiDir` specifies the `wiki` root directory, containing all articles.
- `outputLocation` is optional and specifies a file path and/or name to write the output markdown text to. If this is omitted, it will output to `./result.md`.

### Usage examples

#### .NET Framework (Windows)

```batch
osuWikiTTT.exe <wikiDir> [outputLocation]
```

#### .NET Core (Windows, Linux, macOS)

```batch
dotnet osuWikiTTT.dll <wikiDir> [outputLocation]
```

This is still heavily in development, so it would make sense to check this README again if something stops working!

## How does it work?

