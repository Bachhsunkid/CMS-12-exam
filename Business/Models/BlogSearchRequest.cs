using TrainingTest.Business.Models.Enums;

namespace TrainingTest.Business.Models;

/// <summary>
/// Represents the supported Blog List query-string state. Parsing it in one place keeps
/// invalid values from changing the Search & Navigation query.
/// </summary>
// TODO: Xem lại khi học tới phần search, làm sao để binding trực tiếp text/value vào enum, 
// Kể cả các phần khác như period, quickreads, page. Hiện tại đang parse thủ công trong BlogSearchRequest.Parse
// Nên tránh parse thủ công, mà nên dùng model binding để map trực tiếp query string vào model.
public class BlogSearchRequest
{
    public string Query { get; init; } = string.Empty;
    public string? Tag { get; init; }
    public int? PeriodDays { get; init; }
    public bool QuickReadsOnly { get; init; }
    public BlogSearchSort Sort { get; init; } 
    public int Page { get; init; } = 1;

    public static BlogSearchRequest Parse(IQueryCollection query)
    {
        var rawPeriod = query["period"].ToString();
        int? periodDays = rawPeriod switch
        {
            "7" => 7,
            "30" => 30,
            "90" => 90,
            _ => null
        };

        return new BlogSearchRequest
        {
            Query = query["q"].ToString().Trim(),
            Tag = NormalizeTag(query["tag"].ToString()),
            PeriodDays = periodDays,
            QuickReadsOnly = query["quickreads"] == "1",
            Sort = string.Equals(query["sort"], "newest", StringComparison.OrdinalIgnoreCase)
                ? BlogSearchSort.Newest
                : BlogSearchSort.Relevance,
            Page = int.TryParse(query["page"], out var page) && page > 0 ? page : 1
        };
    }

    private static string? NormalizeTag(string value)
    {
        var tag = value.Trim().ToLowerInvariant();
        return string.IsNullOrWhiteSpace(tag) ? null : tag;
    }
}
