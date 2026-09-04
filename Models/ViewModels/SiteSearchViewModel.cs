using TrainingTest.Business.Models;
using TrainingTest.Models.Pages;

namespace TrainingTest.Models.ViewModels;

public class SiteSearchViewModel : PageViewModel<SiteSearchPage>
{
    public SiteSearchViewModel(SiteSearchPage currentPage) : base(currentPage)
    {
        Page = currentPage;
    }
    public SiteSearchPage Page { get; init; }
    public required SiteSearchRequest Request { get; init; }
    public required IReadOnlyList<SiteSearchResultItem> Results { get; init; }
    public required int TotalResults { get; init; }
    public required int CurrentPageNumber { get; init; }
    public required int TotalPages { get; init; }
}
