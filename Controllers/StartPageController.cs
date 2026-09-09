using Microsoft.AspNetCore.Mvc;
using EPiServer.Filters;
using EPiServer.Web.Routing;
using EPiServer.Globalization;
using TrainingTest.Business.Authoring;
using TrainingTest.Models.Pages;
using TrainingTest.Models.ViewModels;

namespace TrainingTest.Controllers;

public class StartPageController(
    IContentLoader contentLoader,
    IUrlResolver urlResolver,
    IAuthorService authorService) : PageControllerBase<StartPage>
{
    public async Task<IActionResult> Index(StartPage currentPage)
    {
        var language = new LanguageSelector(ContentLanguage.PreferredCulture.Name);
        var blog = contentLoader.GetChildren<BlogListPage>(currentPage.ContentLink, language).FirstOrDefault();
        var search = contentLoader.GetChildren<SiteSearchPage>(currentPage.ContentLink, language).FirstOrDefault();
        var blogUrl = blog is null ? "/" : urlResolver.GetUrl(blog.ContentLink);
        var visitorFilter = new FilterContentForVisitor();
        var blogPost = blog is null
            ? null
            : contentLoader.GetChildren<BlogPostPage>(blog.ContentLink, language)
                .FirstOrDefault(post => !visitorFilter.ShouldFilter(post));

        return View(new StartPageViewModel(currentPage)
        {
            BlogUrl = blogUrl,
            BlogPostUrl = blogPost is null ? blogUrl : urlResolver.GetUrl(blogPost.ContentLink),
            SearchUrl = search is null ? "/" : urlResolver.GetUrl(search.ContentLink),
            AuthorUrl = blog is null ? null : await authorService.GetFirstAuthorUrl(blog),
            NotFoundUrl = blog is null ? "/" : $"{blogUrl.TrimEnd('/')}/author/not-found"
        });
    }
}
