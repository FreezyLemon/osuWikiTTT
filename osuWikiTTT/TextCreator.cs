using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace osuWikiTTT
{
    public class TextCreator
    {
        private string output;

        private ushort indentationAmount = 0;
        private string indentation => string.Concat(Enumerable.Repeat(" ", indentationAmount));

        public string CreateGFM(IEnumerable<Article> rootArticles, string locale)
        {
            Reset();

            var culture = new CultureInfo(locale);

            Write(
$@"<!-- Automatically created by the osuWikiTTT -->

# {culture.EnglishName} Wiki Completion

## Notes <!-- Delete these if you want to -->

1. `- [ ] About (5)` -> The article 'About' has not been translated to {culture.EnglishName} yet, and the english version is 5 lines long.
2. `- [x] Beatmap Editor (20/22)` -> The article 'Beatmap Editor' has been translated to {culture.EnglishName}. The translated version has 20 lines, and the english one has 22.

## Article Listing

"
            );

            GFMLoop(rootArticles, locale);

            return output;
        }

        private void GFMLoop(IEnumerable<Article> articles, string locale)
        {
            foreach (var article in articles)
            {
                var translation = locale != null ? article.Translations.SingleOrDefault(t => t.Language == locale) : null;
                char checkmark = (translation != null) ? 'x' : ' ';

                string toWrite = $"- [{checkmark}] {article.Name}";

                if (locale == "en")
                    toWrite += $" ({translation.LineCount})";
                else
                {
                    // null check only for safety, it should never be null
                    int englishLineCount = article.Translations.SingleOrDefault(t => t.Language == "en")?.LineCount ?? 0;

                    if (translation == null)
                        toWrite += $" ({englishLineCount})";
                    else
                        toWrite += $" ({translation.LineCount}/{englishLineCount})";
                }

                WriteLine(toWrite);

                if (article.SubArticles.Count > 0)
                {
                    indentationAmount += 4;
                    GFMLoop(article.SubArticles, locale);
                    indentationAmount -= 4;
                }       
            }
        }

        private void Reset()
        {
            indentationAmount = 0;
            output = string.Empty;
        }

        private void Write(string text)
        {
            output += indentation + text;
        }

        private void WriteLine(string text) => Write(text + "\n");
    }
}
