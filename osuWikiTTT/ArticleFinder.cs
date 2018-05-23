using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace osuWikiTTT
{
    public static class ArticleFinder
    {
        private static readonly List<Article> allArticles = new List<Article>();
        public static IReadOnlyList<Article> Articles => allArticles;
        public static IReadOnlyList<Article> RootArticles => allArticles.Where(a => a.ParentArticle == null).ToList();

        private static bool isInitialized;
        // string starts with uppercase letter, number, or 'osu!'
        private static readonly Regex firstCharRegex = new Regex("^(?:[A-Z]|\\d|osu!)");

        public static void Initialize(string wikiDirPath, bool countArticles)
        {
            if (isInitialized)
                return;

            isInitialized = true;

            var rootDir = new DirectoryInfo(wikiDirPath);
            if (rootDir.Name != "wiki")
                throw new ArgumentException("Select the 'wiki' directory path!");

            GetArticlesFromDir(rootDir, countArticles);
        }

        private static void GetArticlesFromDir(DirectoryInfo directory, bool countArticles, Article parentArticle = null)
        {
            var subDirectories = directory.GetDirectories().Where(d => firstCharRegex.IsMatch(d.Name)).ToList();

            foreach (var subDir in subDirectories)
            {
                var newArticle = new Article(subDir.Name.Replace('_', ' '));

                foreach (var file in subDir.EnumerateFiles("*.md"))
                {
                    string locale = Path.GetFileNameWithoutExtension(file.Name);

                    if (countArticles)
                    {
                        int lineCount = File.ReadLines(file.FullName).Count();
                        newArticle.AddTranslation(locale, lineCount);
                    }
                    else
                        newArticle.AddTranslation(locale);
                }

                if (parentArticle != null)
                    newArticle.SetParentArticle(parentArticle);

                allArticles.Add(newArticle);

                GetArticlesFromDir(subDir, countArticles, newArticle);
            }
        }
    }
}
