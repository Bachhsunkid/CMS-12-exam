using TrainingTest.Models.Blocks;
using TrainingTest.Models.Pages;

namespace TrainingTest.Models.ViewModels;

public class AuthorPageViewModel
{
    public required BlogListPage Blog { get; init; }
    public required AuthorProfileBlock Author { get; init; }
    public required IReadOnlyList<BlogPostPage> Posts { get; init; }
    public required int CurrentPageNumber { get; init; }
    public required int TotalPages { get; init; }
}
