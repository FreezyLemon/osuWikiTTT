using System;
using System.Globalization;
using System.IO;
using CommandLine;

namespace osuWikiTTT
{
    public class CommandLineArguments
    {
        [Option('d', "directory", Required = true)]
        public string WikiDir { get; set; }

        [Option('o', "output", Required = true)]
        public string OutputFile { get; set; }

        [Option('l', "locale", Required = false)]
        public string Culture { get; set; } = "en";

        [Option('c', "count", Required = false)]
        public bool Count { get; set; } = true;

        [Option("check-outdated", Required = false)]
        public bool OutdatedCheck { get; set; } = false;
    }

    public struct ToolOptions
    {
        public ToolOptions(CommandLineArguments args)
        {
            try
            {
                WikiDir = new DirectoryInfo(args.WikiDir);
            }
            catch (Exception exc)
            {
                throw new ArgumentException("Unknown error when parsing the wiki directory path!", "dir", exc);
            }

            if (!WikiDir.Exists)
                throw new ArgumentException($"Wiki directory couldn't be found at {args.WikiDir}!");

            if (WikiDir.Name != "wiki")
                throw new ArgumentException($"{args.WikiDir} is not the right directory! Choose the 'wiki' directory (not osu-wiki)!");

            try
            {
                OutputFile = new FileInfo(args.OutputFile);
            }
            catch (Exception)
            {
                throw new ArgumentException("Unknown error when parsing the output file name!");
            }

            if (args.Culture.Length != 2)
                throw new ArgumentException("Locale names have to be 2 characters long!");

            try
            {
                Culture = new CultureInfo(args.Culture);
            }
            catch (Exception)
            {
                throw new ArgumentException("Unknown error when parsing the locale name!");
            }

            Count = args.Count;
            OutdatedCheck = args.OutdatedCheck;
        }

        public DirectoryInfo WikiDir { get; }
        public FileInfo OutputFile { get; }
        public CultureInfo Culture { get; }
        public bool Count { get; }
        public bool OutdatedCheck { get; }
    }
}