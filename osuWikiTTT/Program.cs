using Mono.Options;
using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace osuWikiTTT
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ToolOptions options;
            try
            {
                options = ParseOptions(args);
            }
            catch (OptionException exc)
            {
                Console.WriteLine("Error occurred when parsing the input arguments:");
                Console.WriteLine(exc.Message);
                Console.WriteLine("Try the `--help` option for more information");
                return;
            }

            Console.WriteLine($"Trying to read files/folders from {options.WikiDir.FullName}...");
            var finder = new ArticleFinder(options);
            Console.WriteLine($"Found {finder.Articles.Count} articles.");
            Console.WriteLine($"Creating GitHub-flavored Markdown from all articles...");

            string GFMText = new TextCreator(options).CreateGFM(finder.Articles.Where(a => a.ParentArticle == null));
            File.WriteAllText(options.OutputFile.FullName, GFMText);
            Console.WriteLine($"Wrote resulting article structure to {options.OutputFile.FullName}.");
        }

        private static ToolOptions ParseOptions(string[] args)
        {
            DirectoryInfo wikiDir = null;
            FileInfo outputFile = null;
            CultureInfo culture = null;
            ArticleCountType countType = 0;

            bool shouldShowHelp = false;

            var optionSet = new OptionSet
            {
                {
                    "d|dir=", "the path of the wiki directory.", w =>
                    {
                        try
                        {
                            wikiDir = new DirectoryInfo(w);
                        }
                        catch (Exception exc)
                        {
                            throw new OptionException("Unknown error when parsing the wiki directory path!", "dir", exc);
                        }

                        if (!wikiDir.Exists)
                            throw new OptionException($"Wiki directory couldn't be found at {w}!", "dir");

                        if (wikiDir.Name != "wiki")
                            throw new OptionException($"{w} is not the right directory! Choose the 'wiki' directory (not osu-wiki)!", "dir");
                    }
                },
                {
                    "o|output=", "the path of the output file, relative to the current directory. defaults to './result.md'.", o =>
                    {
                        try
                        {
                            outputFile = new FileInfo(o);
                        }
                        catch (Exception exc)
                        {
                            throw new OptionException("Unknown error when parsing the output file name!", "output", exc);
                        }
                    }
                },
                {
                    "l|locale=", "the locale you want to check. Defaults to none, which will show an empty list.", l =>
                    {
                        if (l.Length != 2)
                            throw new OptionException("Locale names have to be 2 characters long!", "locale");

                        try
                        {
                            culture = new CultureInfo(l);
                        }
                        catch (Exception exc)
                        {
                            throw new OptionException("Unknown error when parsing the locale name!", "locale", exc);
                        }
                    }
                },
                {
                    "c|count", "specify this option to count the number of lines per article.", c =>
                    {
                        if (c != null)
                            countType = c == "all" ? ArticleCountType.All : ArticleCountType.Smart;
                        else
                            countType = ArticleCountType.None;
                    }
                },
                { "?|help", "show this help.", h => shouldShowHelp = h != null },
            };

            optionSet.Parse(args);

            if (shouldShowHelp)
            {
                Console.WriteLine("Usage: osuWikiTTT.exe [options]");
                optionSet.WriteOptionDescriptions(Console.Out);

                Environment.Exit(0);
            }

            if (wikiDir == null)
                throw new OptionException("Please specify a wiki directory via the -d option!", "dir");

            if (outputFile == null)
                outputFile = new FileInfo("result.md");

            return new ToolOptions(wikiDir, outputFile, culture, countType);
        }
    }
}
