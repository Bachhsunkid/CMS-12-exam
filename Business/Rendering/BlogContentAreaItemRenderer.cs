using EPiServer.Web;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace TrainingTest.Business.Rendering;

public class BlogContentAreaItemRenderer(IContentAreaLoader contentAreaLoader)
{
    public void RenderLayoutClass(ContentAreaItem contentAreaItem, TagHelperContext context, TagHelperOutput output)
    {
        var tag = contentAreaLoader.LoadDisplayOption(contentAreaItem)?.Tag;
        var layout = tag is "full" or "half" or "card" ? tag : "full";

        output.AddClass($"content-item--{layout}", System.Text.Encodings.Web.HtmlEncoder.Default);
    }
}