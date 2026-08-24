using Microsoft.AspNetCore.Mvc;
using EPiServer.Web.Routing;
using TrainingTest.Business.Helpers;
using TrainingTest.Models.Pages;
using TrainingTest.Models.ViewModels;

namespace TrainingTest.Controllers;

public class BlogPostPageController(UrlResolver urlResolver) : PageControllerBase<BlogPostPage>
{
    public IActionResult Index(BlogPostPage currentPage)
    {
        ViewData["PageCss"] = "/blog-post-page.css";
        SetPageLayout(currentPage);
        
        return View(new BlogPostViewModel
        {
            BlogPost = currentPage,
            ReadingTimeMinutes = currentPage.MainBody.EstimateReadingTime(),
            HeroImageUrl = ContentReference.IsNullOrEmpty(currentPage.HeroImage)
                ? null
                : urlResolver.GetUrl(currentPage.HeroImage),
            AuthorUrl = "/blog/author/jane-doe"
        });
    }
}
