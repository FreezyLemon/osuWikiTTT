using System;
using System.IO;
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
            Console.WriteLine($"Found {finder.Articles.Count.ToString()} articles.");

            Console.WriteLine("Checking ppy/osu-wiki PRs on GitHub for more information...");
            new ApiHelper().CheckPRsAsync(finder.Articles).Wait();

            Console.WriteLine("Writing article list to output file...");
            string path = options.OutputFile.Directory.FullName;
            File.WriteAllText(options.OutputFile.FullName, Jil.JSON.Serialize(finder.Articles));
            File.WriteAllText(Path.Combine(path, "languages.json"), Jil.JSON.Serialize(finder.Languages));
        }
    }
}
