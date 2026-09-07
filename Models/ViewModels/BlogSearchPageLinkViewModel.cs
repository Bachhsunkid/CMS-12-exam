namespace TrainingTest.Models.ViewModels;

public class BlogSearchPageLinkViewModel
{
    public required int PageNumber { get; init; }
    public required string Url { get; init; }
    public bool IsCurrent { get; init; }
}
