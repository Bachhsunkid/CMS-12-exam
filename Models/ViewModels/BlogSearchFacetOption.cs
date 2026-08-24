namespace TrainingTest.Models.ViewModels;

public class BlogSearchFacetOption
{
    public required string Value { get; init; }
    public required string Label { get; init; }
    public required int Count { get; init; }
    public bool IsSelected { get; init; }
}
