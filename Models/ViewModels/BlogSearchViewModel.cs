using TrainingTest.Business.Models;
using TrainingTest.Models.Pages;

namespace TrainingTest.Models.ViewModels;

public class BlogSearchViewModel
{
    public required BlogListPage Blog { get; init; }
    public required BlogSearchRequest Request { get; init; }
    public required IReadOnlyList<BlogPostPage> Posts { get; init; }
    public required IReadOnlyList<BlogSearchFacetOption> TagFacets { get; init; }
    public required IReadOnlyList<BlogSearchFacetOption> PeriodFacets { get; init; }
    public required int CurrentPageNumber { get; init; }
    public required int PageSize { get; init; }
    public required int TotalPosts { get; init; }
    public required int TotalPages { get; init; }
}
