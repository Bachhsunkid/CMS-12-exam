using System.Text.RegularExpressions;
using EPiServer.Find;
using EPiServer.Find.Cms;
using EPiServer.Globalization;
using EPiServer.Web;
using EPiServer.Web.Routing;
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

    public async Task<IContentResult<BlogPostPage>> GetPosts(
        BlogListPage blog,
        AuthorProfileBlock author,
        int pageNumber,
        int pageSize)
    {
        if (string.IsNullOrWhiteSpace(author.FullName) || author.FullName is null)
        {
            return new ContentResult<BlogPostPage>(null, null);
        }

        var matchPost = await client
            .Search<BlogPostPage>()
            .FilterOnCurrentSite()
            .FilterForVisitor()
            .Filter(post => post.ParentLink.ID.Match(blog.ContentLink.ID))
            .Filter(post => post.Author.Match(author.FullName.Trim()))
            .Filter(post => post.PublishDate.InRange(DateTime.MinValue, DateTime.Now))
            .OrderByDescending(post => post.PublishDate)
            .ThenByDescending(post => post.Changed)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .GetContentResultAsync();

        return matchPost;
    }

    public async Task<string?> GetUrl(BlogPostPage post)
    {
        var blog = contentLoader.Get<BlogListPage>(post.ParentLink);
        if (blog is null)
        {
            return null;
        }
        
        if (post.Author is null || !TryGetAuthorProfileFolder(out var authorProfileFolder))
        {
            return null;
        }

        var result = await client
            .Search<AuthorProfileBlock>()
            .Filter(profile => ((IContent)profile).ParentLink.ID.Match(authorProfileFolder.ID))
            .Filter(profile => profile.FullName.Match(post.Author.Trim()))
            .Take(1)
            .GetContentResultAsync();

        var author = result.FirstOrDefault();

        return author is null ? null : GetPartialUrl(blog, author);
    }

    public async Task<Dictionary<string, string>> GetUrls(BlogListPage blog, List<string> authorNames)
    {
        var normalizedAuthorNames = authorNames
            .Select(name => name.Trim())
            .Distinct()
            .ToList();

        if (normalizedAuthorNames.Count == 0 || !TryGetAuthorProfileFolder(out var authorProfileFolder))
        {
            return [];
        }

        var authorProfileBlocks = await client
            .Search<AuthorProfileBlock>()
            .Filter(profile => ((IContent)profile).ParentLink.ID.Match(authorProfileFolder.ID))
            .Filter(profile => profile.FullName.In(normalizedAuthorNames))
            .GetContentResultAsync();

        var urls = new Dictionary<string, string>();
        foreach (var author in authorProfileBlocks)
        {
            var url = GetPartialUrl(blog, author);
            if (!string.IsNullOrEmpty(url) && !string.IsNullOrWhiteSpace(author.FullName))
            {
                urls[author.FullName] = url;
            }
        }

        return urls;
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
