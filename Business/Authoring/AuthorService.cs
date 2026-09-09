using EPiServer.Find;
using EPiServer.Find.Cms;
using EPiServer.Globalization;
using EPiServer.Web;
using EPiServer.Web.Routing;
using TrainingTest.Business.Helpers;
using TrainingTest.Business.Resolvers;
using TrainingTest.Models.Blocks;
using TrainingTest.Models.Pages;

namespace TrainingTest.Business.Authoring;

/// <summary>
/// Finds the current site's author profiles, their posts, and their friendly Blog List URLs.
/// </summary>
public class AuthorService(
    ISiteSettingsResolver siteSettingsResolver,
    IContentLoader contentLoader,
    UrlResolver urlResolver,
    IClient client) : IAuthorService
{
    public async Task<AuthorProfileBlock?> GetBySlug(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug) || !TryGetAuthorProfileFolder(out var authorProfileFolder))
        {
            return null;
        }

        var result = await client
            .Search<AuthorProfileBlock>()
            .Filter(profile => ((IContent)profile).ParentLink.ID.Match(authorProfileFolder.ID))
            .Filter(profile => profile.Slug.Match(slug.Trim()))
            .Take(1)
            .GetContentResultAsync();

        return result.FirstOrDefault();
    }

    public AuthorProfileBlock? GetByReference(ContentReference? authorReference)
    {
        return ContentReference.IsNullOrEmpty(authorReference) ||
               !contentLoader.TryGet(authorReference, out AuthorProfileBlock? author)
            ? null
            : author;
    }

    public async Task<IContentResult<BlogPostPage>> GetPosts(
        BlogListPage blog,
        AuthorProfileBlock author,
        int pageNumber,
        int pageSize)
    {
        var searchNow = SearchQueryCacheHelper.GetSearchNow();

        var matchPost = await client
            .Search<BlogPostPage>()
            .FilterOnCurrentSite()
            .FilterForVisitor()
            .Filter(post => post.ParentLink.ID.Match(blog.ContentLink.ID))
            .Filter(post => post.AuthorRef!.ID.Match(((IContent)author).ContentLink.ID))
            .Filter(post => post.PublishDate.Before(searchNow))
            .OrderByDescending(post => post.PublishDate)
            .ThenByDescending(post => post.Changed)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .GetContentResultAsync();

        return matchPost;
    }

    public string? GetUrl(BlogPostPage post)
    {
        var blog = contentLoader.Get<BlogListPage>(post.ParentLink);
        if (blog is null)
        {
            return null;
        }
        
        var author = GetByReference(post.AuthorRef);
        return author is null ? null : GetPartialUrl(blog, author);
    }

    public Dictionary<int, AuthorReferenceDetails> GetUrls(BlogListPage blog, IEnumerable<ContentReference> authorReferences)
    {
        var uniqueAuthorReferences = authorReferences
            .Where(reference => !ContentReference.IsNullOrEmpty(reference))
            .GroupBy(reference => reference.ID)
            .Select(group => group.First())
            .ToList();

        if (uniqueAuthorReferences.Count == 0)
        {
            return new Dictionary<int, AuthorReferenceDetails>();
        }

        var authors = contentLoader
            .GetItems(uniqueAuthorReferences, new LoaderOptions())
            .OfType<AuthorProfileBlock>();

        var details = authors
            .GroupBy(author => ((IContent)author).ContentLink.ID)
            .ToDictionary(
                group => group.Key,
                group => new AuthorReferenceDetails(
                    group.First().FullName,
                    GetPartialUrl(blog, group.First())));

        return details;
    }

    public async Task<string?> GetFirstAuthorUrl(BlogListPage blog)
    {
        if (!TryGetAuthorProfileFolder(out var authorProfileFolder))
        {
            return null;
        }

        var result = await client
            .Search<AuthorProfileBlock>()
            .Filter(profile => ((IContent)profile).ParentLink.ID.Match(authorProfileFolder.ID))
            .OrderBy(profile => profile.FullName)
            .ThenBy(profile => profile.Slug)
            .Take(1)
            .GetContentResultAsync();

        var author = result.FirstOrDefault();

        return author is null ? null : GetPartialUrl(blog, author);
    }
    
    public string? GetPartialUrl(BlogListPage blog, AuthorProfileBlock author, int pageNumber = 1)
    {
        if (string.IsNullOrWhiteSpace(author.Slug))
        {
            return null;
        }

        var routeData = new AuthorRouteData(blog, author.Slug, pageNumber);
        return urlResolver
            .GetVirtualPathForNonContent(routeData, ContentLanguage.PreferredCulture.Name, null)
            ?.GetUrl();    
    }

    private bool TryGetAuthorProfileFolder(out ContentReference authorProfileFolder)
    {
        var settings = siteSettingsResolver.Get(SiteDefinition.Current, ContentLanguage.PreferredCulture);
        if (settings is null || ContentReference.IsNullOrEmpty(settings.AuthorProfileFolder))
        {
            authorProfileFolder = ContentReference.EmptyReference;
            return false;
        }

        authorProfileFolder = settings.AuthorProfileFolder!;
        return true;
    }
}
