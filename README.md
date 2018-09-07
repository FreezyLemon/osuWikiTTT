# What is this?

The "osu-wiki Translation Tracking Tool", or osuWikiTTT for short, exist to help translators of the wiki keep track of:

- What articles exist in english
- What articles still need to be translated to language X

## What can it do?

This program can check the folder structure of the osu-wiki repository (currently only local) and parse it into a tree of articles.

It can then further use this "tree" to create text output, for example it can create a kind of "tracking list" using [GitHub-flavored Markdown](https://guides.github.com/features/mastering-markdown/#GitHub-flavored-markdown) that roughly looks like this:

```markdown
- [ ] Article 1
- [x] Article 2 <!-- translated in locale x -->
    - [ ] Article 2.1
    - [x] Article 2.2 <!-- translated in locale x -->
    - [ ] Article 2.3
- [ ] Article 3
    - [ ] Article 3.1
```

which will look like this:

- [ ] Article 1
- [x] Article 2 <!-- translated in locale x -->
    - [ ] Article 2.1
    - [x] Article 2.2 <!-- translated in locale x -->
    - [ ] Article 2.3
- [ ] Article 3
    - [ ] Article 3.1

and so on.

## How do I use it?

**Note: This program is currently configured to do exactly one thing: Find all articles in the specified directory and supply a text using GFM that shows all of them in an organized fashion.**

The tool currently only supports .NET Core. It is very easy to add .NET Framework support for Windows users (which is easier to use on Windows), but I'm currently the only one using this. If you want me to add support for the .NET Framework, shoot me a message and I'll do it.

This tool will only take very simple input parameters:

### Usage examples

#### .NET Core (Windows, Linux, macOS)

```bash
dotnet osuWikiTTT.dll [-d|--directory <wikiDir>] [-o|--output <filepath>] [-l|--locale <localestring>] [-c|--count <smart|all>]

dotnet osuWikiTTT.dll /home/FreezyLemon/Desktop/osu-wiki/wiki -l de -o /home/FreezyLemon/Desktop/german-translation-report.md
```

This is still heavily in development, so it would make sense to check this README again if something stops working!

## How does it work?

This section will be added when I find the time to.