using Microsoft.AspNetCore.Mvc;
using TrainingTest.Business.Models;
using TrainingTest.Business.Search;
using TrainingTest.Models.Pages;

namespace TrainingTest.Controllers;

public class BlogListPageController(IBlogSearchService blogSearchService)
    : PageControllerBase<BlogListPage>
{
    public async Task<IActionResult> Index(BlogListPage currentPage, [FromQuery] BlogSearchRequest request)
    {
        var searchResult = await blogSearchService.SearchAsync(currentPage, request);
        return View(searchResult);
    }
}
