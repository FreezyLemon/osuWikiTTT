namespace osuWikiTTT
{
    public enum TranslationStatus
    {
        /// <summary>
        /// A translation exists, but became outdated due to an update of an (usually the english) article.
        /// </summary>
        Outdated = 1,

        /// <summary>
        /// Translation is being created or being updated.
        /// </summary>
        WorkInProgress = 2,

        /// <summary>
        /// A translation is currently in the process of getting reviewed in a github PR.
        /// </summary>
        PROpen = 3,

        /// <summary>
        /// A translation exists and is not outdated
        /// </summary>
        UpToDate = 4,
    }
}
