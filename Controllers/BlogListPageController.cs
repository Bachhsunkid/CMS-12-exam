using Microsoft.AspNetCore.Mvc;
using TrainingTest.Business.Models;
using TrainingTest.Business.Search;
using TrainingTest.Models.Pages;

namespace TrainingTest.Controllers;

public class BlogListPageController(IBlogSearchService blogSearchService)
    : PageControllerBase<BlogListPage>
{
    public async Task<IActionResult> Index(BlogListPage currentPage)
    {
        // SetPageLayout(currentPage);
        var searchResult = await blogSearchService.SearchAsync(currentPage, BlogSearchRequest.Parse(Request.Query));
        return View(searchResult);
    }
}
