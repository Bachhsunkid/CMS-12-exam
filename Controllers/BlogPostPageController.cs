using Microsoft.AspNetCore.Mvc;
using EPiServer.Web.Routing;
using TrainingTest.Business.Helpers;
using TrainingTest.Business.Authoring;
using TrainingTest.Models.Pages;
using TrainingTest.Models.ViewModels;

namespace TrainingTest.Controllers;

public class BlogPostPageController(IUrlResolver urlResolver, IAuthorService authorService) : PageControllerBase<BlogPostPage>
{
    public async Task<IActionResult> Index(BlogPostPage currentPage)
    {
        return View(new BlogPostViewModel(currentPage)
        {
            ReadingTimeMinutes = currentPage.MainBody.EstimateReadingTime(),
            HeroImageUrl = ContentReference.IsNullOrEmpty(currentPage.HeroImage)
                ? null
                : urlResolver.GetUrl(currentPage.HeroImage),
            AuthorUrl = await authorService.GetUrl(currentPage)
        });
    }
}
