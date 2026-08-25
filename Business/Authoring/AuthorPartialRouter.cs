using EPiServer.Core.Routing;
using EPiServer.Core.Routing.Pipeline;
using TrainingTest.Models.Pages;

namespace TrainingTest.Business.Authoring;

/// <summary>
/// Extends a Blog List URL with author profile routes without introducing an Author Page content type.
/// Reference: https://docs.developers.optimizely.com/content-management-system/docs/partial-routing
/// </summary>
public class AuthorPartialRouter : IPartialRouter<BlogListPage, AuthorRouteData>
{
    public object? RoutePartial(BlogListPage content, UrlResolverContext urlResolverContext)
    {
        var authorSegment = urlResolverContext.GetNextSegment();
        var isAuthorSegment = string.Equals(authorSegment.Next.ToString(), "author", StringComparison.OrdinalIgnoreCase);
        if (!isAuthorSegment)
        {
            return null;
        }

        var slugSegment = urlResolverContext.GetNextSegment(authorSegment.Remaining);
        var slug = Uri.UnescapeDataString(slugSegment.Next.ToString()).Trim();
        if (string.IsNullOrWhiteSpace(slug))
        {
            return null;
        }

        var pageNumber = 1;
        var remaining = slugSegment.Remaining;
        if (!remaining.IsEmpty)
        {
            var pageSegment = urlResolverContext.GetNextSegment(remaining);
            if (!TryParsePageNumber(pageSegment.Next.ToString(), out pageNumber) || !pageSegment.Remaining.IsEmpty)
            {
                return null;
            }

            remaining = pageSegment.Remaining;
        }

        urlResolverContext.RemainingSegments = remaining;
        return new AuthorRouteData(content, slug, pageNumber);
    }

    public PartialRouteData GetPartialVirtualPath(AuthorRouteData routeData, UrlGeneratorContext urlGeneratorContext)
    {
        return new PartialRouteData
        {
            BasePathRoot = routeData.Blog.ContentLink,
            PartialVirtualPath = AuthorRouteData.BuildRelativePath(routeData.Slug, routeData.PageNumber)
        };
    }

    private static bool TryParsePageNumber(string segment, out int pageNumber)
    {
        const string prefix = "page-";
        pageNumber = 1;

        return segment.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)
               && int.TryParse(segment[prefix.Length..], out pageNumber)
               && pageNumber > 0;
    }
}
