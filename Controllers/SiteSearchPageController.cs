using Microsoft.AspNetCore.Mvc;
using TrainingTest.Business.Models;
using TrainingTest.Business.Search;
using TrainingTest.Models.Pages;

namespace TrainingTest.Controllers;

public class SiteSearchPageController(ISiteSearchService siteSearchService)
    : PageControllerBase<SiteSearchPage>
{
    public async Task<IActionResult> Index(SiteSearchPage currentPage, [FromQuery] SiteSearchRequest request)
    {
        var siteSearchModel = await siteSearchService.SearchAsync(currentPage, request);
        return View(siteSearchModel);
    }
}
