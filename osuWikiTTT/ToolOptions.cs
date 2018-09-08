using System;
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
    }

    public struct ToolOptions
    {
        public ToolOptions(CommandLineArguments args)
        {
            try
            {
                WikiDir = new DirectoryInfo(args.WikiDir);
            }
            catch (Exception)
            {
                throw new ArgumentException("Unknown error when parsing the wiki directory path!");
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
        }

        public DirectoryInfo WikiDir { get; }
        public FileInfo OutputFile { get; }
    }
}