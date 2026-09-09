using TrainingTest.Models.Blocks;
using TrainingTest.Models.Pages;

namespace TrainingTest.Models.ViewModels;

public class AuthorPageViewModel : PageViewModel<BlogListPage>
{
    public AuthorPageViewModel(BlogListPage currentPage) : base(currentPage)
    {
        Blog = currentPage;
    }

    public BlogListPage Blog { get; init; }
    public required AuthorProfileBlock Author { get; init; }
    public required IReadOnlyList<BlogPostPage> Posts { get; init; }
    public required PagingViewModelBase Paging { get; init; } 
}
