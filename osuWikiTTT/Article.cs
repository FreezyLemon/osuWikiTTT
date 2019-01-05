using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;

namespace osuWikiTTT
{
    /// <summary>
    /// An osu!wiki article, in all languages that are available.
    /// Initializes to only contain the english version.
    /// </summary>
    public class Article
    {
        public Article(string name, string firstLanguage = "en")
        {
            Name = name;
        }

        internal void AddTranslation(string locale, int lineCount, TranslationStatus status, params int[] prNumbers)
            => Translations.Add(locale, new Translation(this, locale, lineCount, status, prNumbers));

        internal void SetParentArticle(Article parentArticle)
        {
            parentArticle.AddSubArticle(this);
            ParentArticle = parentArticle;
        }

        internal void AddSubArticle(Article subArticle)
        {
            subArticle.ParentArticle = this;
            SubArticles.Add(subArticle);
        }

        [DataMember]
        public string Name { get; }

        [IgnoreDataMember]
        public string FullName
        {
            get
            {
                string temp = Name;
                var article = this;
                while (article.ParentArticle != null)
                {
                    temp = article.ParentArticle.Name + '/' + temp;
                    article = article.ParentArticle;
                }

                return temp;
            }
        }

        [IgnoreDataMember]
        public Article ParentArticle { get; private set; }
        
        [DataMember]
        public List<Article> SubArticles { get; } = new List<Article>();

        [DataMember]
        public Dictionary<string, Translation> Translations { get; } = new Dictionary<string, Translation>();

        #region Equals overload

        public override bool Equals(object obj)
        {
            return obj is Article other
                && Name == other.Name
                && SubArticles.SequenceEqual(other.SubArticles)
                && Translations.SequenceEqual(other.Translations);
        }

        public static bool operator ==(Article a1, Article a2)
        {
            if (a1 is null)
                return a2 is null;

            return a1.Equals(a2);
        }
        public static bool operator !=(Article a1, Article a2) => !(a1 == a2);

        #endregion

        public override string ToString()
        {
            string result = Name;
            var current = this;
            while (current.ParentArticle != null)
            {
                current = current.ParentArticle;
                result = current.Name + "/" + result;
            }

            return result;
        }

        public class Translation
        {
            internal Translation()
            {
            }

            internal Translation(Article article, string locale, int lineCount, TranslationStatus status, int[] prNumbers)
            {
                Article = article;
                Language = locale;
                LineCount = lineCount;
                Status = status;

                if (!(prNumbers is null) && prNumbers.Length > 0)
                {
                    PullRequestNumbers.AddRange(prNumbers);
                }

                LastChanged = getLastChanged();
            }

            [IgnoreDataMember]
            public Article Article { get; }

            [IgnoreDataMember]
            public string Language { get; }

            [DataMember]
            public int LineCount { get; }

            [DataMember]
            public TranslationStatus Status { get; set; }

            [DataMember]
            public List<int> PullRequestNumbers { get; } = new List<int>();

            [DataMember]
            public long LastChanged { get; set; }

            [IgnoreDataMember]
            public string Filename => Language + ".md";

            public override string ToString() => $"[{Language}] {Article.Name}";

            private long getLastChanged()
            {
                string filename = Path.Combine(Program.Options.WikiDir.FullName, Article.FullName.Replace(' ', '_'), Filename);

                var p = new Process
                {
                    StartInfo = new ProcessStartInfo("git", $"log -n 1 --format=%at {filename}")
                    {
                        WorkingDirectory = Program.Options.WikiDir.FullName,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true,
                    }
                };

                p.Start();

                string timestamp = p.StandardOutput.ReadLine();
                string test = p.StandardOutput.ReadToEnd();
                return long.TryParse(timestamp, out long unix) ?
                    unix :
                    0;
            }
        }
    }
}
