using System.Collections.Generic;
using System.Linq;

namespace osuWikiTTT
{
    public class TextCreator
    {
        private string output;

        private ushort indentationAmount = 0;
        private string indentation => string.Concat(Enumerable.Repeat(" ", indentationAmount));

        public string CreateGFM(IEnumerable<Article> rootArticles, string locale = null)
        {
            Reset();
            GFMLoop(rootArticles, locale);

            return output;
        }

        private void GFMLoop(IEnumerable<Article> articles, string locale)
        {
            foreach (var article in articles)
            {
                char checkmark = (locale != null & article.Translations.Any(t => t.Language == locale)) ? 'x' : ' ';

                WriteLine($"- [{checkmark}] {article.Name}");

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
