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

        // [DE], [FR], ... -> [??]
        private readonly Regex _languageTagRegex = new Regex(@"\[[a-zA-Z]{2}\]", RegexOptions.Compiled | RegexOptions.IgnoreCase);

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

            foreach (var pr in translationPRs)
            {
                var match = _languageTagRegex.Match(pr.Title);

                if (!match.Success)
                {
                    continue;
                }

                var localeString = match.Groups[0].Value.TrimStart('[').TrimEnd(']').ToLowerInvariant();

                bool articleHandled = false;
                if (match.Index == 0)
                {
                    // If the PR has a title like "[ES] Rules", we can find the article just from that.
                    // A lot of the current PRs have a title like this, so this is worth a shot
                    var potentialArticleTitle = pr.Title.Substring(4).Trim();

                    var article = articles.FirstOrDefault(a => a.Name == potentialArticleTitle);

                    if (!(article is null))
                    {
                        if (article.Translations.TryGetValue(localeString, out var translation))
                        {
                            if (pr.State.Value == ItemState.Open)
                            {
                                translation.Status = TranslationStatus.PROpen;
                                translation.PullRequestNumbers.Add(pr.Number);
                            }
                            else if (pr.PullRequest.Merged)
                            {
                                translation.PullRequestNumbers.Add(pr.Number);
                            }
                        }
                        else
                        {
                            article.AddTranslation(localeString, 0, TranslationStatus.PROpen, pr.Number);
                        }

                        articleHandled = true;
                    }
                }
                
                if (!articleHandled)
                {
                    var files = await _client.PullRequest.Files(osu_wiki_repo_id, pr.Number);
                    foreach (var file in files)
                    {
                        
                    }
                }
            }
        }
    }
}
