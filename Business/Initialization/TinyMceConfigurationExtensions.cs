using EPiServer.Cms.TinyMce.Core;
using TrainingTest.Models.Pages;

namespace TrainingTest.Business.Initialization;

public static class TinyMceConfigurationExtensions
{
    public static IServiceCollection AddTinyMceConfiguration(this IServiceCollection services)
    {
        // reference: https://docs.developers.optimizely.com/content-management-system/docs/tinymce-editor
        services.Configure<TinyMceConfiguration>(configuration =>
        {
            configuration.For<BlogPostPage>(page => page.MainBody)
                .ContentCss("/blog-post-page.css")
                .AppendToolbar("styles", 0)
                .StyleFormats(
                    new { title = "Primary Button", selector = "a", attributes = new { @class = "btn btn-primary" } },
                    new { title = "Secondary Button", selector = "a", attributes = new { @class = "btn btn-secondary" } },
                    new { title = "Lead Paragraph", selector = "p", attributes = new { @class = "lead" } },
                    new { title = "Pull Quote", selector = "blockquote", attributes = new { @class = "pull-quote" } });

            configuration.For<BlogListPage>(page => page.Intro)
                .Menubar(string.Empty)
                .Toolbar("bold italic | epi-link | undo redo")
                .Height(200)
                .Resize(TinyMceResize.None);
        });

        return services;
    }
}