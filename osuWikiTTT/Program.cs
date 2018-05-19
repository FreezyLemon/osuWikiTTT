using Mono.Options;
using System;
using System.Collections.Generic;
using System.IO;

namespace osuWikiTTT
{
    public class Program
    {
        private static string wikiDirectory;
        private static string outputFilename;
        private static string locale;

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
            ArticleFinder.Initialize(wikiDirectory);
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
                { "<>|d|dir=", "the path of the wiki directory.", w =>
                    {
                        if (string.IsNullOrWhiteSpace(w))
                            throw new OptionException("Please enter a valid wiki directory!", "<>|d|dir=");

                        if (!new DirectoryInfo(w).Exists)
                            throw new OptionException($"Wiki directory couldn't be found at {w}!", "<>|d|dir=");

                        wikiDirectory = w;
                    }
                },
                { "o|output=", "the path of the output file, relative to the current directory. defaults to './result.md'.", o => outputFilename = o },
                { "l|locale=", "the locale you want to check. Does *not* check for locale if empty.", l =>
                    {
                        if (string.IsNullOrWhiteSpace(l))
                            throw new OptionException("Please enter a valid locale string!", "l|locale=");

                        if (l.Length != 2)
                            throw new OptionException("Locale names have to be 2 characters long!", "l|locale=");

                        locale = l;
                    }
                },
            };

            var result = options.Parse(args);

            // set defaults here
            if (string.IsNullOrEmpty(outputFilename))
                outputFilename = "result.md";

            return result;
        }
    }
}
