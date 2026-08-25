using EPiServer.Web;
using EPiServer.Web.Routing;
using EPiServer.Web.Routing.Matching;
using Microsoft.AspNetCore.Mvc;
using TrainingTest.Business.Authoring;
using TrainingTest.Business.Helpers;
using TrainingTest.Business.Resolvers;
using TrainingTest.Models.ViewModels;

namespace TrainingTest.Controllers;

/// <summary>
/// Renders the non-page object supplied by AuthorPartialRouter below a BlogListPage URL.
/// Reference: https://docs.developers.optimizely.com/content-management-system/docs/example-of-news-partial-routing
/// </summary>
public class AuthorPageController(
    IAuthorService authorService,
    IPageLayoutResolver pageLayoutResolver,
    UrlResolver urlResolver) : Controller, IRenderTemplate<AuthorRouteData>
{
    public IActionResult Index()
    {
        var routeData = HttpContext.Features.Get<IContentRouteFeature>()?.RoutedContentData.PartialRoutedObject as AuthorRouteData;
        if (routeData is null)
        {
            return NotFound();
        }

        var author = authorService.GetBySlug(routeData.Slug);
        if (author is null)
        {
            return RenderNotFound(routeData.Blog);
        }

        var posts = authorService.GetPosts(routeData.Blog, author);
        var totalPages = PaginationHelper.GetTotalPages(posts.Count, Constants.DefaultPageSize);
        var currentPage = PaginationHelper.NormalizePage(routeData.PageNumber, totalPages);

        ViewData["Title"] = author.FullName;
        ViewData["PageCss"] = "/author.css";
        ViewData["PageLayout"] = pageLayoutResolver.Resolve(routeData.Blog);

        return View(new AuthorPageViewModel
        {
            Blog = routeData.Blog,
            Author = author,
            Posts = posts
                .Skip(PaginationHelper.GetSkip(currentPage, Constants.DefaultPageSize))
                .Take(Constants.DefaultPageSize)
                .ToList(),
            CurrentPageNumber = currentPage,
            TotalPages = totalPages
        });
    }

    private ViewResult RenderNotFound(Models.Pages.BlogListPage blog)
    {
        Response.StatusCode = StatusCodes.Status404NotFound;
        ViewData["Title"] = "Page not found";
        ViewData["PageCss"] = "/not-found.css";
        ViewData["PageLayout"] = pageLayoutResolver.Resolve(blog);

        return View("~/Views/Shared/NotFound.cshtml", new NotFoundViewModel
        {
            BlogUrl = urlResolver.GetUrl(blog.ContentLink)
        });
    }
}
