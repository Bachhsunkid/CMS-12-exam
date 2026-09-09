using TrainingTest.Models.Pages;

namespace TrainingTest.Business.Authoring;

/// <summary>
/// Represents the author and requested page parsed from the URL below a Blog List page.
/// The router deliberately does not look up the author: the controller needs the same route
/// data for both a valid profile and a visitor-facing 404 response.
/// </summary>
public record AuthorRouteData(BlogListPage Blog, string Slug, int PageNumber)
{
    public static string BuildRelativePath(string slug, int pageNumber = 1)
    {
        var encodedSlug = Uri.EscapeDataString(slug);
        return pageNumber > 1
            ? $"author/{encodedSlug}/page-{pageNumber}"
            : $"author/{encodedSlug}";
    }
}
