using TrainingTest.Models.Pages;

namespace TrainingTest.Models.ViewModels;

public class BlogPostListItemViewModel
{
    public required BlogPostPage Post { get; init; }
    public string? AuthorUrl { get; init; }
}
