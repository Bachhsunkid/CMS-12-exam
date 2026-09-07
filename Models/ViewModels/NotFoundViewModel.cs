using TrainingTest.Models.Pages;

namespace TrainingTest.Models.ViewModels;

public class NotFoundViewModel(BlogListPage currentPage) : PageViewModel<BlogListPage>(currentPage)
{
    public required string BlogUrl { get; init; }
}
