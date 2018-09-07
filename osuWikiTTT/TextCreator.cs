using System.Collections.Generic;
using System.Linq;

namespace osuWikiTTT
{
    public class TextCreator
    {
        private readonly ToolOptions _options;
        private readonly string _locale;

        private string _output;
        private ushort _indentationAmount = 0;

        protected string Indentation => string.Concat(Enumerable.Repeat(" ", _indentationAmount));

        public TextCreator(ToolOptions options)
        {
            _options = options;
            _locale = options.Culture?.TwoLetterISOLanguageName ?? string.Empty;
        }

        public string CreateGFM(IEnumerable<Article> rootArticles)
        {
            Reset();

            Write("<!-- Automatically created by the osuWikiTTT -->\n\n");

            Write("## Article Listing\n\n");

            GFMLoop(rootArticles);

            return _output;
        }

        private void GFMLoop(IEnumerable<Article> articles)
        {
            foreach (var article in articles)
            {
                var translation = article.Translations.SingleOrDefault(t => t.Language == _locale);
                char checkmark = (translation != null) ? 'x' : ' ';

                string line = $"- [{checkmark}] {article.Name}";

                if (!string.IsNullOrEmpty(_locale) && _options.Count)
                {
                    var enTranslation = _locale != "en" ? article.Translations.SingleOrDefault(t => t.Language == "en") : translation;
                    int enLineCount = enTranslation?.LineCount ?? 0;

                    if (translation == null)
                        line += $" (en: {enLineCount})";
                    else if (!translation.IsOutdated)
                        line += $" (en: {enLineCount}, {translation.Language}: {translation.LineCount})";
                    else
                        line += $" (en: {enLineCount}, {translation.Language}: {translation.LineCount}, **outdated**)";
                }

                WriteLine(line);

                if (article.SubArticles.Any())
                {
                    _indentationAmount += 4;
                    GFMLoop(article.SubArticles);
                    _indentationAmount -= 4;
                }       
            }
        }

        private void Reset()
        {
            _indentationAmount = 0;
            _output = string.Empty;
        }

        private void Write(string text)
        {
            _output += Indentation + text;
        }

        private void WriteLine(string text) => Write(text + "\n");
    }
}
