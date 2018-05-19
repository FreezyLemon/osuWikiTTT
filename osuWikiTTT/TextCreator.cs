using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace osuWikiTTT
{
    public class TextCreator
    {
        private string output;

        private ushort indentationAmount = 0;
        private string indentation => string.Concat(Enumerable.Repeat(" ", indentationAmount));

        public string CreateGFM(IEnumerable<Article> rootArticles)
        {
            Reset();
            GFMLoop(rootArticles);

            return output;
        }

        private void GFMLoop(IEnumerable<Article> articles)
        {
            foreach (var article in articles)
            {
                WriteLine($"- [ ] {article.Name}");

                if (article.SubArticles.Count > 0)
                {
                    indentationAmount += 2;
                    GFMLoop(article.SubArticles);
                    indentationAmount -= 2;
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
