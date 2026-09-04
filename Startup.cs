using EPiServer.Cms.Shell;
using EPiServer.Cms.UI.AspNetIdentity;
using EPiServer.Core.Routing;
using EPiServer.Scheduler;
using EPiServer.Web.Routing;
using TrainingTest.Business.Blog;
using TrainingTest.Business.Authoring;
using TrainingTest.Business.Initialization;
using TrainingTest.Business.Resolvers;
using TrainingTest.Business.Search;
using TrainingTest.Extensions;

namespace TrainingTest;

public class Startup
{
    private readonly IWebHostEnvironment _webHostingEnvironment;

    public Startup(IWebHostEnvironment webHostingEnvironment)
    {
        _webHostingEnvironment = webHostingEnvironment;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        if (_webHostingEnvironment.IsDevelopment())
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", Path.Combine(_webHostingEnvironment.ContentRootPath, "App_Data"));

            services.Configure<SchedulerOptions>(options => options.Enabled = false);
        }

        services
            .AddCmsAspNetIdentity<ApplicationUser>()
            .AddCms()
            .AddTrainingTest()
            .AddAdminUserRegistration()
            .AddEmbeddedLocalization<Startup>();
        
        services.AddFind();
        services.AddBlogDisplayOptions();
        services.AddBlogTinyMceConfiguration();

        services.AddScoped<IBlogSearchService, BlogSearchService>();
        services.AddScoped<ISiteSearchService, SiteSearchService>();
        services.AddScoped<IAuthorService, AuthorService>();
        services.AddSingleton<IPartialRouter, AuthorPartialRouter>();
        services.AddSingleton<IBlogSeedDataProvider, BlogSeedDataProvider>();
        services.AddSingleton<ISiteSettingsResolver, SiteSettingsResolver>();
        services.AddSingleton<IPageLayoutResolver, PageLayoutResolver>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapContent();
        });
    }
}
