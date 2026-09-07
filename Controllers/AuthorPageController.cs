using EPiServer.Web;
using EPiServer.Web.Routing;
using EPiServer.Web.Routing.Matching;
using Microsoft.AspNetCore.Mvc;
using TrainingTest.Business;
using TrainingTest.Business.Authoring;
using TrainingTest.Business.Models;
using TrainingTest.Business.Resolvers;
using TrainingTest.Models.Pages;
using TrainingTest.Models.ViewModels;

namespace TrainingTest.Controllers;

/// <summary>
/// Renders the non-page object supplied by AuthorPartialRouter below a BlogListPage URL.
/// Reference: https://docs.developers.optimizely.com/content-management-system/docs/example-of-news-partial-routing
/// </summary>
public class AuthorPageController(
    IAuthorService authorService,
    IPageLayoutResolver pageLayoutResolver,
    IUrlResolver urlResolver) : Controller, IRenderTemplate<AuthorRouteData>, IModifyLayout
{
    public async Task<IActionResult> Index()
    {
        var routeData = HttpContext.Features.Get<IContentRouteFeature>()?.RoutedContentData.PartialRoutedObject as AuthorRouteData;
        if (routeData is null)
        {
            return NotFound();
        }

        var author = await authorService.GetBySlug(routeData.Slug);
        if (author is null)
        {
            return RenderNotFound(routeData.Blog);
        }

        var pageSize = Constants.DefaultPageSize;
        var matchedPost = await authorService
            .GetPosts(routeData.Blog, author, routeData.PageNumber, pageSize);

        return View(new AuthorPageViewModel(routeData.Blog)
        {
            Author = author,
            Posts = matchedPost.Items.ToList(),
            Paging = new PagingViewModelBase
            {
                CurrentPage = routeData.PageNumber,
                PageSize = pageSize,
                TotalItems = matchedPost.TotalMatching
            }
        });
    }

    private ViewResult RenderNotFound(BlogListPage blog)
    {
        Response.StatusCode = StatusCodes.Status404NotFound;

        return View("~/Views/Shared/NotFound.cshtml", new NotFoundViewModel(blog)
        {
            BlogUrl = urlResolver.GetUrl(blog.ContentLink)
        });
    }

    public void ModifyLayout(LayoutModel layoutModel)
    {
        var routeData = HttpContext.Features.Get<IContentRouteFeature>()?.RoutedContentData.PartialRoutedObject as AuthorRouteData;
        var blog = routeData?.Blog;
        if (blog is null)
        {
            return;
        }

        layoutModel.Header = pageLayoutResolver.ResolveHeader(blog);
        layoutModel.Footer = pageLayoutResolver.ResolveFooter(blog);
    }
}
