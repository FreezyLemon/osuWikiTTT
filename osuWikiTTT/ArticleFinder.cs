using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace osuWikiTTT
{
    public class ArticleFinder
    {
        public IReadOnlyList<Article> Articles
        {
            get
            {
                if (!_allArticles.Any())
                {
                    ReloadArticlesFrom(_options.WikiDir);
                }

                return _allArticles;
            }
        }

        // string starts with uppercase letter, number, or 'osu!'
        private static readonly Regex articleDirNameRegex = new Regex("^(?:[A-Z]|\\d|osu!)");

        private readonly ToolOptions _options;
        private readonly List<Article> _allArticles = new List<Article>();

        public ArticleFinder(ToolOptions options)
        {
            _options = options;
        }

        public void ReloadArticlesFrom(DirectoryInfo wikiDir)
        {
            reloadArticlesFrom(wikiDir);
        }

        private void reloadArticlesFrom(DirectoryInfo directory, Article parentArticle = null)
        {
            var subDirectories = directory.GetDirectories().Where(d => articleDirNameRegex.IsMatch(d.Name));

            foreach (var subDir in subDirectories)
            {
                var newArticle = new Article(subDir.Name.Replace('_', ' '));

                foreach (var file in subDir.EnumerateFiles("*.md"))
                {
                    string fileLocale = Path.GetFileNameWithoutExtension(file.Name);

                    bool isOutdated = false;

                    var allLines = File.ReadAllLines(file.FullName);

                    if (allLines[0].Trim() == "---" &&
                        allLines[1].Trim() == "outdated: true" &&
                        allLines[2].Trim() == "---")
                    {
                        isOutdated = true;
                    }

                    newArticle.AddTranslation(fileLocale, allLines.Length, isOutdated);
                }

                if (parentArticle != null)
                    newArticle.SetParentArticle(parentArticle);

                _allArticles.Add(newArticle);

                reloadArticlesFrom(subDir, newArticle);
            }
        }
    }
}
