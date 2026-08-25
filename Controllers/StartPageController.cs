using Microsoft.AspNetCore.Mvc;
using EPiServer.Web.Routing;
using System.Globalization;
using TrainingTest.Business.Authoring;
using TrainingTest.Models.Pages;
using TrainingTest.Models.ViewModels;

namespace TrainingTest.Controllers;

public class StartPageController(
    IContentLoader contentLoader,
    UrlResolver urlResolver,
    IAuthorService authorService) : PageControllerBase<StartPage>
{
    public IActionResult Index(StartPage currentPage)
    {
        ViewData["PageCss"] = "/index.css";
        SetPageLayout(currentPage);

        var language = new LanguageSelector(CultureInfo.CurrentUICulture.Name);
        var blog = contentLoader.GetChildren<BlogListPage>(currentPage.ContentLink, language).FirstOrDefault();
        var search = contentLoader.GetChildren<SiteSearchPage>(currentPage.ContentLink, language).FirstOrDefault();
        var blogUrl = blog is null ? "/" : urlResolver.GetUrl(blog.ContentLink);
        var blogPost = blog is null
            ? null
            : contentLoader.GetChildren<BlogPostPage>(blog.ContentLink, language).FirstOrDefault();

        return View(new StartPageViewModel
        {
            BlogUrl = blogUrl,
            BlogPostUrl = blogPost is null ? blogUrl : urlResolver.GetUrl(blogPost.ContentLink),
            SearchUrl = search is null ? "/" : urlResolver.GetUrl(search.ContentLink),
            AuthorUrl = blog is null ? null : authorService.GetFirstAuthorUrl(blog),
            NotFoundUrl = blog is null ? "/" : $"{blogUrl.TrimEnd('/')}/author/not-found"
        });
    }
}
