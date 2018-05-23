using Mono.Options;
using System;
using System.Collections.Generic;
using System.IO;

namespace osuWikiTTT
{
    public class Program
    {
        private static string wikiDirectory;
        private static string outputFilename = "result.md";
        private static string locale = "en";
        private static bool countArticles = false;

        public static void Main(string[] args)
        {
            try
            {
                ParseOptions(args);
            }
            catch (OptionException exc)
            {
                Console.WriteLine("Error occurred when parsing the input arguments:");
                Console.WriteLine(exc.Message);
                // Console.WriteLine("Try the `--help` option for more information"); doesn't exist yet
                return;
            }

            Console.WriteLine($"Trying to read files/folders from {wikiDirectory}...");
            ArticleFinder.Initialize(wikiDirectory, countArticles);
            Console.WriteLine($"Found {ArticleFinder.Articles.Count} articles.");
            Console.WriteLine($"Creating GitHub-flavored Markdown from all articles...");

            string GFMText = new TextCreator().CreateGFM(ArticleFinder.RootArticles, locale);
            File.WriteAllText(outputFilename, GFMText);
            Console.WriteLine($"Wrote resulting article structure to {outputFilename}");
        }

        private static List<string> ParseOptions(string[] args)
        {
            var options = new OptionSet
            {
                {
                    "<>|d|dir=", "the path of the wiki directory.", w =>
                    {
                        if (string.IsNullOrWhiteSpace(w))
                            throw new OptionException("Please enter a valid wiki directory!", "<>|d|dir=");

                        if (!new DirectoryInfo(w).Exists)
                            throw new OptionException($"Wiki directory couldn't be found at {w}!", "<>|d|dir=");

                        wikiDirectory = w;
                    }
                },
                {
                    "o|output=", "the path of the output file, relative to the current directory. defaults to './result.md'.", o => outputFilename = o },
                {
                    "l|locale=", "the locale you want to check. Default is 'en'.", l =>
                    {
                        if (string.IsNullOrWhiteSpace(l))
                            return;

                        if (l.Length != 2)
                            throw new OptionException("Locale names have to be 2 characters long!", "l|locale=");

                        locale = l;
                    }
                },
                { "c|count", "specify this option to count the number of lines per article.", c => countArticles = c != null },
            };

            var result = options.Parse(args);

            if (string.IsNullOrEmpty(wikiDirectory))
                throw new OptionException("Please specify at least one argument pointing to the wiki directory!", "<>|d|dir");

            return result;
        }
    }
}
