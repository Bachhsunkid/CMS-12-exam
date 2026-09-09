using EPiServer.Web.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TrainingTest.Models.Pages;
using TrainingTest.Models.ViewModels;

namespace TrainingTest.Business;

public class PageContextActionFilter(PageViewContextFactory contextFactory) : IResultFilter
{
    public void OnResultExecuting(ResultExecutingContext context)
    {
        var controller = context.Controller as Controller;
        var viewModel = controller?.ViewData.Model;
        
        if(viewModel is IPageViewModel<SitePageData> model)
        {
            var currentContentLink = context.HttpContext.GetContentLink();
            
            var layoutModel = model.Layout ?? contextFactory.CreateLayoutModel(currentContentLink);
            
            if (context.Controller is IModifyLayout layoutController)
            {
                layoutController.ModifyLayout(layoutModel);
            }
            
            model.Layout = layoutModel;
        }
    }

    public void OnResultExecuted(ResultExecutedContext context)
    {
        
    }
}