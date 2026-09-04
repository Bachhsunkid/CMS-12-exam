using Microsoft.AspNetCore.Mvc;
using TrainingTest.Business.Models;
using TrainingTest.Business.Search;
using TrainingTest.Models.Pages;

namespace TrainingTest.Controllers;

public class SiteSearchPageController(ISiteSearchService siteSearchService)
    : PageControllerBase<SiteSearchPage>
{
    public async Task<IActionResult> Index(SiteSearchPage currentPage)
    {
        // SetPageLayout(currentPage);
        
        var siteSearchModel = await siteSearchService.SearchAsync(currentPage, SiteSearchRequest.Parse(Request.Query));

        return View(siteSearchModel);
    }
}
