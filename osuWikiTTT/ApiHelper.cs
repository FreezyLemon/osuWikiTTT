using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Octokit;

namespace osuWikiTTT
{
    public class ApiHelper
    {
        private const long osu_wiki_repo_id = 66639319;

        // [DE], [FR], ...
        private readonly Regex _languageTagRegex = new Regex(@"\[[a-zA-Z]{2}\]", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        // id.md, en.md, fr.md, ...
        private readonly Regex _markdownFilenameRegex = new Regex(@"^[a-z]{2}\.md$", RegexOptions.Compiled);

        private readonly GitHubClient _client;

        public ApiHelper()
        {
            var header = new ProductHeaderValue("osu-wiki-status-tracker");
            string token = Environment.GetEnvironmentVariable("GITHUB_API_TOKEN");

            _client = new GitHubClient(header);

            if (!string.IsNullOrEmpty(token))
            {
                _client.Credentials = new Credentials(token);
            }
        }

        public async Task CheckPRsAsync(IEnumerable<Article> articles)
        {
            var req = new RepositoryIssueRequest
            {
                Filter = IssueFilter.All,
                State = ItemStateFilter.All,
                SortProperty = IssueSort.Created,
                SortDirection = SortDirection.Ascending,
            };
            req.Labels.Add("translations");

            var translationIssues = await _client.Issue.GetAllForRepository(osu_wiki_repo_id, req);
            var translationPRs = translationIssues.Where(i => i.PullRequest != null).ToList();

            foreach (var prAsIssue in translationPRs)
            {
                var realPr = await _client.PullRequest.Get(osu_wiki_repo_id, prAsIssue.Number);

                var match = _languageTagRegex.Match(prAsIssue.Title);

                if (!match.Success)
                {
                    continue;
                }

                if (prAsIssue.State.Value != ItemState.Open && !realPr.Merged)
                {
                    continue;
                }

                var localeString = match.Value.TrimStart('[').TrimEnd(']').ToLowerInvariant();

                bool articleHandled = false;
                if (match.Index == 0)
                {
                    // If the PR has a title like "[ES] Rules", we can find the article just from that.
                    // A lot of the current PRs have a title like this, so this is worth a shot
                    var potentialArticleTitle = prAsIssue.Title.Substring(4).Trim();

                    var article = articles.FirstOrDefault(a => a.Name == potentialArticleTitle);

                    if (!(article is null))
                    {
                        if (article.Translations.TryGetValue(localeString, out var translation))
                        {
                            if (prAsIssue.State.Value == ItemState.Open)
                            {
                                translation.Status = TranslationStatus.PROpen;
                                translation.PullRequestNumbers.Add(prAsIssue.Number);
                            }
                            else if (prAsIssue.PullRequest.Merged)
                            {
                                translation.PullRequestNumbers.Add(prAsIssue.Number);
                            }
                        }
                        else if (prAsIssue.State.Value == ItemState.Open)
                        {
                            article.AddTranslation(localeString, 0, TranslationStatus.PROpen, prAsIssue.Number);
                        }

                        articleHandled = true;
                    }
                }

                if (articleHandled)
                {
                    continue;
                }

                var files = await _client.PullRequest.Files(osu_wiki_repo_id, prAsIssue.Number);
                foreach (var file in files)
                {
                    var split = file.FileName.Split('/');
                    if (split.Length < 3 || split[0] != "wiki" || !_markdownFilenameRegex.IsMatch(split.Last()))
                    {
                        continue;
                    }

                    IEnumerable<Article> parentArticles = articles;
                    Article article = null;
                    for (int i = 1; i < split.Length - 1; ++i)
                    {
                        string articleName = split[i].Replace('_', ' ');
                        article = parentArticles.FirstOrDefault(a => a.Name == articleName);

                        if (article is null)
                        {
                            break;
                        }

                        parentArticles = article.SubArticles;
                    }

                    if (!(article is null))
                    {
                        articleHandled = true;

                        if (!article.Translations.TryGetValue(localeString, out var translation))
                        {
                            if (prAsIssue.State.Value == ItemState.Open)
                            {
                                article.AddTranslation(localeString, 0, TranslationStatus.PROpen, prAsIssue.Number);
                                translation = article.Translations[localeString];
                            }
                            else
                            {
                                continue;
                            }
                        }

                        translation.PullRequestNumbers.Add(prAsIssue.Number);
                            
                        if (prAsIssue.State.Value == ItemState.Open)
                        {
                            translation.Status = TranslationStatus.PROpen;
                        }
                    }
                }
            }
        }
    }
}
