using System;
using System.Globalization;
using System.IO;
using System.Linq;
using CommandLine;

namespace osuWikiTTT
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<CommandLineArguments>(args)
                .WithParsed(options => Run(new ToolOptions(options)))
                .WithNotParsed(errs => Environment.Exit(1));
        }

        private static void Run(ToolOptions options)
        {
            Console.WriteLine($"Trying to read files/folders from {options.WikiDir.FullName}...");
            var finder = new ArticleFinder(options);
            Console.WriteLine($"Found {finder.Articles.Count} articles.");
            Console.WriteLine($"Creating GitHub-flavored Markdown from all articles...");

            string GFMText = new TextCreator(options).CreateGFM(finder.Articles.Where(a => a.ParentArticle == null));
            File.WriteAllText(options.OutputFile.FullName, GFMText);
            Console.WriteLine($"Wrote resulting article structure to {options.OutputFile.FullName}.");
        }
    }
}
