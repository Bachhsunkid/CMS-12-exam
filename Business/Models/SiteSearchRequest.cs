using TrainingTest.Business.Models.Enums;

namespace TrainingTest.Business.Models;

public class SiteSearchRequest
{
    public string Query { get; init; } = string.Empty;
    public SiteSearchType Type { get; init; } = SiteSearchType.All;
    public int Page { get; init; } = 1;

    public static SiteSearchRequest Parse(IQueryCollection query)
    {
        var type = query["type"].ToString().ToLowerInvariant() switch
        {
            "page" => SiteSearchType.Page,
            "blog-post" => SiteSearchType.BlogPost,
            "document" => SiteSearchType.Document,
            _ => SiteSearchType.All
        };

        var page = int.TryParse(query["page"], out var parsedPage) && parsedPage > 0
            ? parsedPage
            : 1;

        return new SiteSearchRequest
        {
            Query = query["q"].ToString().Trim(),
            Type = type,
            Page = page
        };
    }
}
