using EPiServer.DataAccess;
using EPiServer.Globalization;
using EPiServer.PlugIn;
using EPiServer.Scheduler;
using EPiServer.Security;
using EPiServer.Web;
using TrainingTest.Business.Blog;
using TrainingTest.Business.Models;
using TrainingTest.Business.Resolvers;
using TrainingTest.Models.Blocks;
using TrainingTest.Models.Pages;

namespace TrainingTest.Jobs;

[ScheduledPlugIn(
    DisplayName = "Seed blog content",
    Description = "Creates the deterministic training blog content when it is missing.",
    GUID = "50b5e458-8d22-44e6-a213-c3b2a7bfdc90")]
public class SeedBlogContentJob : ScheduledJobBase
{
    private readonly ISiteDefinitionRepository _siteDefinitionRepository;
    private readonly ISiteSettingsResolver _siteSettingsResolver;
    private readonly IContentLoader _contentLoader;
    private readonly IContentRepository _contentRepository;
    private readonly IBlogSeedDataProvider _blogSeedDataProvider;
    private volatile bool _stopSignaled;

    public SeedBlogContentJob(
        ISiteDefinitionRepository siteDefinitionRepository,
        ISiteSettingsResolver siteSettingsResolver,
        IContentLoader contentLoader,
        IContentRepository contentRepository,
        IBlogSeedDataProvider blogSeedDataProvider)
    {
        _siteDefinitionRepository = siteDefinitionRepository;
        _siteSettingsResolver = siteSettingsResolver;
        _contentLoader = contentLoader;
        _contentRepository = contentRepository;
        _blogSeedDataProvider = blogSeedDataProvider;
        IsStoppable = true;
    }

    // Seeds only the existing sites that already have settings and a Blog List page.
    // Reference: https://docs.developers.optimizely.com/content-management-system/docs/scheduled-jobs
    public override string Execute()
    {
        _stopSignaled = false;

        var result = new BlogContentSeedResult();
        BlogSeedData seedData;
        try
        {
            seedData = _blogSeedDataProvider.LoadSeedData();
        }
        catch (Exception exception)
        {
            return $"Seed blog content failed to load JSON data: {exception.Message}";
        }

        var now = DateTime.Now;
        foreach (var site in _siteDefinitionRepository.List())
        {
            if (_stopSignaled)
            {
                result.Stopped = true;
                return result.ToSummary();
            }

            try
            {
                SeedSite(site, seedData, now, result);
            }
            catch (Exception exception)
            {
                result.Failed++;
                result.Messages.Add($"{site.Name}: {exception.Message}");
            }
        }

        return result.ToSummary();
    }

    public override void Stop()
    {
        _stopSignaled = true;
        OnStatusChanged("Stopping after the current content item.");
    }

    private void SeedSite(SiteDefinition site, BlogSeedData seedData, DateTime now, BlogContentSeedResult result)
    {
        // A missing settings page or Blog List is configuration to fix, not content this job should create.
        var settings = _siteSettingsResolver.Get(site, ContentLanguage.PreferredCulture);
        if (settings is null)
        {
            result.Messages.Add($"{site.Name}: Site settings were not found; no content was seeded.");
            return;
        }

        var blogList = _contentLoader.GetChildren<BlogListPage>(site.StartPage).SingleOrDefault();
        if (blogList is null)
        {
            result.Messages.Add($"{site.Name}: Blog list page was not found; no content was seeded.");
            return;
        }

        var authorFolder = GetOrCreateAuthorFolder(settings, result);
        if (authorFolder is null || _stopSignaled)
        {
            return;
        }

        var authors = SeedAuthors(authorFolder.ContentLink, seedData.Authors, result);
        SeedPosts(blogList.ContentLink, authors, seedData.Posts, now, result);
        UpdateAuthorPostCounts(authorFolder.ContentLink, blogList.ContentLink, result);
    }

    private ContentFolder? GetOrCreateAuthorFolder(SiteSettingsPage settings, BlogContentSeedResult result)
    {
        // The author folder is the one seed-owned prerequisite; its reference is persisted on Site Settings.
        // Reference: https://docs.developers.optimizely.com/content-management-system/docs/creating-and-editing-content
        if (!ContentReference.IsNullOrEmpty(settings.AuthorProfileFolder) &&
            _contentLoader.TryGet(settings.AuthorProfileFolder, out ContentFolder? folder) &&
            folder is not null)
        {
            return folder;
        }

        if (_stopSignaled)
        {
            result.Stopped = true;
            return null;
        }

        var newFolder = _contentRepository.GetDefault<ContentFolder>(ContentReference.GlobalBlockFolder);
        newFolder.Name = "Author profiles";
        var folderReference = _contentRepository.Save(newFolder, SaveAction.Publish, AccessLevel.NoAccess);

        var writableSettings = (SiteSettingsPage)settings.CreateWritableClone();
        writableSettings.AuthorProfileFolder = folderReference;
        _contentRepository.Save(writableSettings, SaveAction.Publish, AccessLevel.NoAccess);
        return _contentLoader.Get<ContentFolder>(folderReference);
    }

    private IReadOnlyDictionary<string, AuthorProfileBlock> SeedAuthors(
        ContentReference folderLink,
        IEnumerable<AuthorSeedData> authorSeeds,
        BlogContentSeedResult result)
    {
        var authors = new Dictionary<string, AuthorProfileBlock>(StringComparer.OrdinalIgnoreCase);
        foreach (var seed in authorSeeds)
        {
            if (_stopSignaled)
            {
                result.Stopped = true;
                break;
            }

            var existing = _contentLoader.GetChildren<AuthorProfileBlock>(folderLink)
                .SingleOrDefault(author => string.Equals(author.Slug, seed.Slug, StringComparison.OrdinalIgnoreCase));

            if (existing is not null)
            {
                authors.Add(seed.Slug, existing);
                continue;
            }

            var author = _contentRepository.GetDefault<AuthorProfileBlock>(folderLink);
            AsContent(author).Name = seed.Name;
            author.FullName = seed.Name;
            author.Slug = seed.Slug;
            author.Bio = seed.Bio;
            var authorLink = _contentRepository.Save(AsContent(author), SaveAction.Publish, AccessLevel.NoAccess);
            authors.Add(seed.Slug, _contentLoader.Get<AuthorProfileBlock>(authorLink));
            result.AuthorsCreated++;
        }

        return authors;
    }

    private void SeedPosts(
        ContentReference blogListLink,
        IReadOnlyDictionary<string, AuthorProfileBlock> authors,
        IEnumerable<BlogPostSeedData> postSeeds,
        DateTime now,
        BlogContentSeedResult result)
    {
        // URL segments are stable seed keys, so a second run does not create or overwrite a post.
        var existingByRouteSegment = _contentLoader.GetChildren<BlogPostPage>(blogListLink)
            .Where(post => !string.IsNullOrWhiteSpace(post.URLSegment))
            .GroupBy(post => post.URLSegment, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase);

        foreach (var seed in postSeeds)
        {
            OnStatusChanged($"Seeding {seed.RouteSegment}.");
            if (_stopSignaled)
            {
                result.Stopped = true;
                return;
            }

            if (existingByRouteSegment.ContainsKey(seed.RouteSegment))
            {
                result.PostsUnchanged++;
                continue;
            }

            if (!authors.TryGetValue(seed.AuthorSlug, out var author))
            {
                result.Failed++;
                result.Messages.Add($"{seed.RouteSegment}: author slug '{seed.AuthorSlug}' could not be resolved.");
                continue;
            }

            try
            {
                var post = _contentRepository.GetDefault<BlogPostPage>(blogListLink);
                post.Name = seed.Title;
                post.URLSegment = seed.RouteSegment;
                post.Title = seed.Title;
                post.Summary = seed.Summary;
                post.MainBody = new XhtmlString($"<p>{seed.Summary}</p><p>{seed.Body}</p>");
                post.PublishDate = now.AddDays(seed.PublishOffsetDays);
                post.Author = author.FullName;
                post.Tags = seed.Tags;
                _contentRepository.Save(post, SaveAction.Publish, AccessLevel.NoAccess);
                result.PostsCreated++;
            }
            catch (Exception exception)
            {
                result.Failed++;
                result.Messages.Add($"{seed.RouteSegment}: {exception.Message}");
            }
        }
    }

    private void UpdateAuthorPostCounts(ContentReference folderLink, ContentReference blogListLink, BlogContentSeedResult result)
    {
        var posts = _contentLoader.GetChildren<BlogPostPage>(blogListLink)
            .Where(post => post.Status == VersionStatus.Published)
            .ToList();

        foreach (var author in _contentLoader.GetChildren<AuthorProfileBlock>(folderLink))
        {
            if (_stopSignaled)
            {
                result.Stopped = true;
                return;
            }

            var count = posts.Count(post => string.Equals(post.Author, author.FullName, StringComparison.OrdinalIgnoreCase));
            if (author.PostCount == count)
            {
                continue;
            }

            var writableAuthor = (AuthorProfileBlock)author.CreateWritableClone();
            writableAuthor.PostCount = count;
            _contentRepository.Save(AsContent(writableAuthor), SaveAction.Publish, AccessLevel.NoAccess);
        }
    }

    private static IContent AsContent(AuthorProfileBlock author)
    {
        return author as IContent
            ?? throw new InvalidOperationException("A shared author profile block must be hosted as CMS content.");
    }

}
