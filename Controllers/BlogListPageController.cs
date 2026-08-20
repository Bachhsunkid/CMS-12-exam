using Microsoft.AspNetCore.Mvc;
using TrainingTest.Business.Blog;
using TrainingTest.Models.Pages;

namespace TrainingTest.Controllers;

public class BlogListPageController(IBlogPostQueryService blogPostQueryService)
    : PageControllerBase<BlogListPage>
{
    public IActionResult Index(BlogListPage currentPage, int page = 1)
    {
        ViewData["PageCss"] = "/blog-landing-page.css";
        
        return View(blogPostQueryService.GetPosts(currentPage, page));
    }
}
