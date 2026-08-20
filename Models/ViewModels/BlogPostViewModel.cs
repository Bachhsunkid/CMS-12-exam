using TrainingTest.Models.Pages;

namespace TrainingTest.Models.ViewModels;

public class BlogPostViewModel
{
    public required BlogPostPage BlogPost { get; init; }
    public required int ReadingTimeMinutes { get; init; }
    public string? HeroImageUrl { get; init; }
    public string? AuthorUrl { get; init; }
}
