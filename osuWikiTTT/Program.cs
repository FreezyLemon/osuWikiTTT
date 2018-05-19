using System;
using System.IO;
using System.Reflection;

namespace osuWikiTTT
{
    class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length == 0)
                return;

            Console.WriteLine($"Trying to read files/folders from {args[0]}...");
            ArticleFinder.Initialize(args[0]);
            Console.WriteLine($"Found {ArticleFinder.Articles.Count} articles.");
            Console.WriteLine($"Creating GitHub-flavored Markdown from all articles...");
            string GFMText = new TextCreator().CreateGFM(ArticleFinder.RootArticles);
            string filename = args.Length > 1 ? args[1] : "result.md";
            File.WriteAllText(filename, GFMText);
            Console.WriteLine($"Wrote resulting article structure to {filename}");
        }
    }
}
