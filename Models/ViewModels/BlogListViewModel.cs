using TrainingTest.Models.Pages;

namespace TrainingTest.Models.ViewModels;

public class BlogListViewModel
{
    public required BlogListPage Blog { get; init; }
    public required IReadOnlyList<BlogPostPage> Posts { get; init; }
    public required int CurrentPageNumber { get; init; }
    public required int PageSize { get; init; }
    public required int TotalPosts { get; init; }
    public required int TotalPages { get; init; }
}