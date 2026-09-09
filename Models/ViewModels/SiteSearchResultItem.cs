using TrainingTest.Business.Models.Enums;

namespace TrainingTest.Models.ViewModels;

public record SiteSearchResultItem
{
    public required SiteSearchType Type { get; init; }
    public required string Title { get; init; }
    public required string Excerpt { get; init; }
    public required string Url { get; init; }
    public bool HasHighlightedExcerpt { get; init; }
}
