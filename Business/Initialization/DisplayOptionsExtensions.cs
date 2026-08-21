using EPiServer.Web;
using TrainingTest.Business.Rendering;

namespace TrainingTest.Business.Initialization;

public static class DisplayOptionsExtensions
{
    public static IServiceCollection AddBlogDisplayOptions(this IServiceCollection services)
    {
        // Defines how editors choose Full, Half, or Card rendering for items in a content area.
        // Reference: https://docs.developers.optimizely.com/content-management-system/docs/display-options
        services.Configure<DisplayOptions>(options =>
        {
            options.Add("full", "Full width layout", Globals.Layouts.FullWidth, string.Empty, "epi-icon__layout--full");
            options.Add("half", "Half width layout", Globals.Layouts.HalfWidth, string.Empty, "epi-icon__layout--half");
            options.Add("card", "Card layout", Globals.Layouts.Card, string.Empty, "epi-icon__layout--narrow");
        });

        services.AddSingleton<BlogContentAreaItemRenderer>();

        return services;
    }
}
