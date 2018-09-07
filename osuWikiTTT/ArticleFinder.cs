using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace osuWikiTTT
{
    public class ArticleFinder
    {
        public IReadOnlyList<Article> Articles => _allArticles;

        // string starts with uppercase letter, number, or 'osu!'
        private static readonly Regex firstCharRegex = new Regex("^(?:[A-Z]|\\d|osu!)");

        private readonly ToolOptions _options;
        private readonly List<Article> _allArticles = new List<Article>();

        public ArticleFinder(ToolOptions options)
        {
            _options = options;
            GetArticlesFromDir(options.WikiDir);
        }

        private void GetArticlesFromDir(DirectoryInfo directory, Article parentArticle = null)
        {
            var subDirectories = directory.GetDirectories().Where(d => firstCharRegex.IsMatch(d.Name));

            foreach (var subDir in subDirectories)
            {
                var newArticle = new Article(subDir.Name.Replace('_', ' '));

                foreach (var file in subDir.EnumerateFiles("*.md"))
                {
                    string fileLocale = Path.GetFileNameWithoutExtension(file.Name);

                    if (_options.Count || _options.OutdatedCheck)
                    {
                        var allLines = File.ReadAllLines(file.FullName);
                        int? length = null;
                        bool isOutdated = false;
                        
                        if (_options.Count && fileLocale == _options.Culture?.TwoLetterISOLanguageName || fileLocale == "en")
                        {
                            length = allLines.Length;
                        }

                        if (_options.OutdatedCheck &&
                            allLines[0].Trim() == "---" &&
                            allLines[1].Trim() == "outdated: true" &&
                            allLines[2].Trim() == "---")
                        {
                            isOutdated = true;
                        }

                        newArticle.AddTranslation(fileLocale, length, isOutdated);
                    }
                }

                if (parentArticle != null)
                    newArticle.SetParentArticle(parentArticle);

                _allArticles.Add(newArticle);

                GetArticlesFromDir(subDir, newArticle);
            }
        }
    }
}
