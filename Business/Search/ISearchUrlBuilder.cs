using TrainingTest.Business.Models.Enums;
using TrainingTest.Models.ViewModels;

namespace TrainingTest.Business.Search;

public interface ISearchUrlBuilder
{
    string GetBlogListSearchUrl(BlogSearchViewModel model, string? tag, int? periodDays, int page = 1);
    string GetSiteSearchUrl(SiteSearchViewModel model, SiteSearchType type, int page = 1);
    string GetSiteSearchTypeLabel(SiteSearchType type);
}
