using System.Globalization;
using EPiServer.Find;
using EPiServer.Find.Cms;
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
    IUrlResolver urlResolver,
    IClient client) : IAuthorService
{
    public AuthorProfileBlock? GetBySlug(string slug)
    {
        return string.IsNullOrWhiteSpace(slug)
            ? null
            : GetProfiles().FirstOrDefault(profile => string.Equals(profile.Slug, slug, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<AuthorPostSearchResult> GetPosts(
        BlogListPage blog,
        AuthorProfileBlock author,
        int pageNumber,
        int pageSize)
    {
        if (string.IsNullOrWhiteSpace(author.FullName) || pageSize < 1)
        {
            return new AuthorPostSearchResult([], 0);
        }

        var authorName = author.FullName.Trim();
        var skip = Math.Max(0, pageNumber - 1) * pageSize;
        var result = await client
            .Search<BlogPostPage>()
            .FilterOnCurrentSite()
            .FilterForVisitor()
            .Filter(post => post.ParentLink.ID.Match(blog.ContentLink.ID))
            .Filter(post => post.Author.Match(authorName))
            .Filter(post => post.PublishDate.InRange(DateTime.MinValue, DateTime.Now))
            .OrderByDescending(post => post.PublishDate)
            .ThenByDescending(post => post.Changed)
            .Skip(skip)
            .Take(pageSize)
            .GetContentResultAsync();

        return new AuthorPostSearchResult(result.Items.ToList(), result.TotalMatching);
    }

    public string? GetUrl(BlogPostPage post)
    {
        return contentLoader.TryGet(post.ParentLink, out BlogListPage? blog) && blog is not null
            ? GetUrl(blog, post.Author)
            : null;
    }

    public string? GetUrl(BlogListPage blog, string? authorName)
    {
        var author = string.IsNullOrWhiteSpace(authorName)
            ? null
            : GetProfiles().FirstOrDefault(profile =>
                string.Equals(profile.FullName?.Trim(), authorName.Trim(), StringComparison.OrdinalIgnoreCase));

        return author is null ? null : GetUrl(blog, author);
    }

    public string? GetFirstAuthorUrl(BlogListPage blog)
    {
        var author = GetProfiles().FirstOrDefault();
        return author is null ? null : GetUrl(blog, author);
    }

    public string GetUrl(BlogListPage blog, AuthorProfileBlock author, int pageNumber = 1)
    {
        return $"{urlResolver.GetUrl(blog.ContentLink).TrimEnd('/')}/{AuthorRouteData.BuildRelativePath(author.Slug ?? string.Empty, pageNumber)}";
    }

    private IEnumerable<AuthorProfileBlock> GetProfiles()
    {
        var settings = siteSettingsResolver.Get(SiteDefinition.Current, CultureInfo.CurrentUICulture);
        if (settings is null || ContentReference.IsNullOrEmpty(settings.AuthorProfileFolder))
        {
            return [];
        }

        return contentLoader.GetChildren<AuthorProfileBlock>(
            settings.AuthorProfileFolder,
            new LanguageSelector(CultureInfo.CurrentUICulture.Name));
    }
}
