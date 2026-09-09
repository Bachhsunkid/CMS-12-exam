using EPiServer.DataAccess;
using EPiServer.PlugIn;
using EPiServer.Scheduler;
using EPiServer.Security;
using EPiServer.Web;
using TrainingTest.Business.Helpers;
using TrainingTest.Business.Models;
using TrainingTest.Models.Pages;

namespace TrainingTest.Jobs;

[ScheduledPlugIn(
    DisplayName = "Normalize blog tags",
    Description = "Trims, lowercases, and deduplicates tags on blog posts.",
    GUID = "1d00a3ee-b9d4-4684-b7a0-af97988f31b6")]
public class NormalizeBlogTagsJob : ScheduledJobBase
{
    private readonly ISiteDefinitionRepository _siteDefinitionRepository;
    private readonly IContentLoader _contentLoader;
    private readonly IContentRepository _contentRepository;
    private volatile bool _stopSignaled;

    public NormalizeBlogTagsJob(
        ISiteDefinitionRepository siteDefinitionRepository,
        IContentLoader contentLoader,
        IContentRepository contentRepository)
    {
        _siteDefinitionRepository = siteDefinitionRepository;
        _contentLoader = contentLoader;
        _contentRepository = contentRepository;
        IsStoppable = true;
    }

    // Normalizes every blog post below each Blog List
    // Reference: https://docs.developers.optimizely.com/content-management-system/docs/scheduled-jobs
    public override string Execute()
    {
        _stopSignaled = false;
        var result = new BlogTagNormalizationResult();

        foreach (var site in _siteDefinitionRepository.List())
        {
            var blogList = _contentLoader.GetChildren<BlogListPage>(site.StartPage).SingleOrDefault();
            if (blogList is null)
            {
                result.Messages.Add($"{site.Name}: Blog list page was not found.");
                continue;
            }

            foreach (var post in _contentLoader.GetChildren<BlogPostPage>(blogList.ContentLink))
            {
                OnStatusChanged($"Normalizing {post.Name}.");
                if (_stopSignaled)
                {
                    result.Stopped = true;
                    return result.ToSummary();
                }

                Normalize(post, result);
            }
        }

        return result.ToSummary();
    }

    public override void Stop()
    {
        _stopSignaled = true;
        OnStatusChanged("Stopping after the current blog post.");
    }

    private void Normalize(BlogPostPage post, BlogTagNormalizationResult result)
    {
        // Save only a changed tag sequence, avoiding an unnecessary CMS version on repeated runs.
        // Reference: https://docs.developers.optimizely.com/content-management-system/v13.0.0-CMS/docs/icontentrepository
        result.Scanned++;
        try
        {
            var normalizedTags = TagNormalizationHelper.Normalize(post.Tags);
            var currentTags = post.Tags ?? [];
            if (currentTags.SequenceEqual(normalizedTags, StringComparer.Ordinal))
            {
                result.Unchanged++;
                return;
            }

            var writablePost = (BlogPostPage)post.CreateWritableClone();
            writablePost.Tags = normalizedTags.ToList();
            _contentRepository.Save(writablePost, SaveAction.Publish, AccessLevel.NoAccess);
            result.Changed++;
        }
        catch (Exception exception)
        {
            result.Failed++;
            result.Messages.Add($"{post.Name}: {exception.Message}");
        }
    }

    private IEnumerable<TContent> GetDescendants<TContent>(ContentReference parentLink)
        where TContent : IContent
    {
        foreach (var child in _contentLoader.GetChildren<IContent>(parentLink))
        {
            if (child is TContent typedContent)
            {
                yield return typedContent;
            }

            foreach (var descendant in GetDescendants<TContent>(child.ContentLink))
            {
                yield return descendant;
            }
        }
    }
}
