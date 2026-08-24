using TrainingTest.Business.Models;
using TrainingTest.Models.Pages;
using TrainingTest.Models.ViewModels;

namespace TrainingTest.Business.Search;

public interface ISiteSearchService
{
    Task<SiteSearchViewModel> SearchAsync(SiteSearchPage searchPage, SiteSearchRequest request);
}
