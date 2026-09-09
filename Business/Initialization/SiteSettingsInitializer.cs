using EPiServer.DataAccess;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.Security;
using EPiServer.Web;
using TrainingTest.Models.Pages;

namespace TrainingTest.Business.Initialization;

[InitializableModule]
[ModuleDependency(typeof(InitializationModule))]
public class SiteSettingsInitializer : IInitializableModule
{
    private IContentLoader? _contentLoader;
    private IContentRepository? _contentRepository;
    private ISiteDefinitionRepository? _siteDefinitionRepository;

    public void Initialize(InitializationEngine context)
    {
        _siteDefinitionRepository = context.Locate.Advanced
            .GetRequiredService<ISiteDefinitionRepository>();
        _contentLoader = context.Locate.Advanced.GetRequiredService<IContentLoader>();
        _contentRepository = context.Locate.Advanced.GetRequiredService<IContentRepository>();

        // The site tree is safe to query only after all CMS modules have initialized.
        context.InitComplete += OnInitComplete;
    }

    public void Uninitialize(InitializationEngine context)
    {
    }
    
    private void OnInitComplete(object? sender, EventArgs e)
    {
        CreateMissingSiteSettings(sender, e);
    }

    private void CreateMissingSiteSettings(object? sender, EventArgs eventArgs)
    {
        foreach (var site in _siteDefinitionRepository!.List())
        {
            if (!IsExistedStartPageInstance(_contentLoader!, site.StartPage))
            {
                continue;
            }

            if (_contentLoader!.GetChildren<SiteSettingsPage>(site.StartPage).Any())
            {
                // The page is developer-owned; never overwrite settings already configured by an editor.
                continue;
            }

            // SiteSettingsPage is hidden from New Page, so this is the only creation path.
            var settings = _contentRepository!.GetDefault<SiteSettingsPage>(site.StartPage);
            settings.Name = "Site settings";
            settings.SiteName = "Opti blog";
            _contentRepository.Save(settings, SaveAction.Publish, AccessLevel.NoAccess);
        }
    }

    private static bool IsExistedStartPageInstance(IContentLoader contentLoader, ContentReference startPage)
    {
        if (ContentReference.IsNullOrEmpty(startPage))
        {
            return false;
        }

        return contentLoader.TryGet<StartPage>(startPage, out _);
    }
}
