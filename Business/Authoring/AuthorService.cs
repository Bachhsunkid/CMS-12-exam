using System.Globalization;
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
    UrlResolver urlResolver) : IAuthorService
{
    public AuthorProfileBlock? GetBySlug(string slug)
    {
        return string.IsNullOrWhiteSpace(slug)
            ? null
            : GetProfiles().FirstOrDefault(profile => string.Equals(profile.Slug, slug, StringComparison.OrdinalIgnoreCase));
    }

    public IReadOnlyList<BlogPostPage> GetPosts(BlogListPage blog, AuthorProfileBlock author)
    {
        if (string.IsNullOrWhiteSpace(author.FullName))
        {
            return [];
        }

        var authorName = author.FullName.Trim();

        return contentLoader
            .GetChildren<BlogPostPage>(blog.ContentLink, new LanguageSelector(CultureInfo.CurrentUICulture.Name))
            .Where(post => post.Status == VersionStatus.Published &&
                           post.PublishDate <= DateTime.Now &&
                           string.Equals(post.Author?.Trim(), authorName, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(post => post.PublishDate)
            .ThenByDescending(post => post.Changed)
            .ToList();
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

    public string GetUrl(BlogListPage blog, AuthorProfileBlock author, int pageNumber = 1)
    {
        return $"{urlResolver.GetUrl(blog.ContentLink).TrimEnd('/')}/{AuthorRouteData.BuildRelativePath(author.Slug ?? string.Empty, pageNumber)}";
    }

    private IReadOnlyList<AuthorProfileBlock> GetProfiles()
    {
        var settings = siteSettingsResolver.Get(SiteDefinition.Current, CultureInfo.CurrentUICulture);
        if (settings is null || ContentReference.IsNullOrEmpty(settings.AuthorProfileFolder))
        {
            return [];
        }

        return contentLoader
            .GetChildren<AuthorProfileBlock>(settings.AuthorProfileFolder, new LanguageSelector(CultureInfo.CurrentUICulture.Name))
            .ToList();
    }
}
