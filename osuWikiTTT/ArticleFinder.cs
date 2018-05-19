﻿using System;
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

        public static void Initialize(string wikiDirPath)
        {
            if (isInitialized)
                return;

            isInitialized = true;

            var rootDir = new DirectoryInfo(wikiDirPath);
            if (rootDir.Name != "wiki")
                throw new ArgumentException("Select the 'wiki' directory path!");

            GetArticlesFromDir(rootDir);
        }

        private static void GetArticlesFromDir(DirectoryInfo directory, Article parentArticle = null)
        {
            var subDirectories = directory.GetDirectories().Where(d => firstCharRegex.IsMatch(d.Name)).ToList();

            foreach (var subDir in subDirectories)
            {
                var newArticle = new Article(subDir.Name.Replace('_', ' '));

                foreach (var file in subDir.EnumerateFiles("*.md"))
                    newArticle.AddTranslation(Path.GetFileNameWithoutExtension(file.Name));

                if (parentArticle != null)
                    newArticle.SetParentArticle(parentArticle);

                allArticles.Add(newArticle);

                GetArticlesFromDir(subDir, newArticle);
            }
        }
    }
}
