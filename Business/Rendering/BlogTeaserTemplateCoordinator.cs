using EPiServer.ServiceLocation;
using EPiServer.Web.Mvc;
using TrainingTest.Models.Blocks;

namespace TrainingTest.Business.Rendering;

[ServiceConfiguration(typeof(IViewTemplateModelRegistrator))]
public class BlogTeaserTemplateCoordinator : IViewTemplateModelRegistrator
{
    public void Register(TemplateModelCollection templates)
    {
        templates.Add(typeof(BlogTeaserBlock), new TemplateModel
        {
            Name = "BlogTeaserBlockDefault",
            Tags = [Globals.Layouts.FullWidth, Globals.Layouts.HalfWidth],
            AvailableWithoutTag = true,
            Path = "~/Views/Shared/Blocks/BlogTeaserBlock.cshtml"
        });

        templates.Add(typeof(BlogTeaserBlock), new TemplateModel
        {
            Name = "BlogTeaserBlockCard",
            Tags = [Globals.Layouts.Card],
            AvailableWithoutTag = false,
            Path = "~/Views/Shared/Blocks/BlogTeaserBlockCard.cshtml"
        });
    }
}
