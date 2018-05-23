using System.Globalization;
using System.IO;

namespace osuWikiTTT
{
    public struct ToolOptions
    {
        public ToolOptions(DirectoryInfo wikiDir, FileInfo outputFile, CultureInfo culture, ArticleCountType countType)
        {
            WikiDir = wikiDir;
            OutputFile = outputFile;
            Culture = culture;
            CountType = countType;
        }

        public DirectoryInfo WikiDir { get; private set; }
        public FileInfo OutputFile { get; private set; }
        public CultureInfo Culture { get; private set; }
        public ArticleCountType CountType { get; private set; }
    }
}