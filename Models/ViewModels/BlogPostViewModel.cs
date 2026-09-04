using TrainingTest.Models.Pages;

namespace TrainingTest.Models.ViewModels;

public class BlogPostViewModel : PageViewModel<BlogPostPage>
{
    public BlogPostViewModel(BlogPostPage currentPage) : base(currentPage)
    {
        BlogPost = currentPage;
    }

    public BlogPostPage BlogPost { get; init; }
    public required int ReadingTimeMinutes { get; init; }
    public string? HeroImageUrl { get; init; }
    public string? AuthorUrl { get; init; }
}
