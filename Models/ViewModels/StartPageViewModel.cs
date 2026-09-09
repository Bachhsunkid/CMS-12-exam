using TrainingTest.Models.Pages;

namespace TrainingTest.Models.ViewModels;

public class StartPageViewModel(StartPage currentPage) : PageViewModel<StartPage>(currentPage)
{
    public required string BlogUrl { get; init; }
    public required string BlogPostUrl { get; init; }
    public required string SearchUrl { get; init; }
    public string? AuthorUrl { get; init; }
    public required string NotFoundUrl { get; init; }
}
