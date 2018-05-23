using System.Collections.Generic;
using System.Linq;

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

        internal void AddTranslation(string locale, int lineCount) => Translations.Add(new Translation(locale, lineCount, this));

        internal void AddTranslation(string locale) => Translations.Add(new Translation(locale, this));

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

        public string Name { get; }
        public Article ParentArticle { get; private set; }
        public List<Article> SubArticles { get; } = new List<Article>();
        public List<Translation> Translations { get; } = new List<Translation>();

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
            internal Translation(string language, Article article)
            {
                Language = language;
                LineCount = null;
                Article = article;
            }

            internal Translation(string language, int lineCount, Article article)
            {
                Language = language;
                LineCount = lineCount;
                Article = article;
            }

            public Article Article { get; }
            public string Language { get; }
            public int? LineCount { get; } = 0;
            public string Filename => Language + ".md";

            public override string ToString() => $"[{Language}] {Article.Name}";
        }
    }
}
