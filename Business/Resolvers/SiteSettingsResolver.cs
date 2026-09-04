using System.Globalization;
using EPiServer.Web;
using EPiServer.Framework.Cache;
using TrainingTest.Models.Pages;

namespace TrainingTest.Business.Resolvers;

public class SiteSettingsResolver : ISiteSettingsResolver, IDisposable
{
    // Finds the shared SiteSettingsPage for the current site, so pages do not load settings themselves.
    // Reference: https://docs.developers.optimizely.com/content-management-system/docs/object-caching
    private const string CacheMasterKey = "TrainingTest:SiteSettings";
    private readonly IContentLoader _contentLoader;
    private readonly ISynchronizedObjectInstanceCache _cache; // no need cache manually, as IContentLoader is already cached.
    private readonly IContentEvents _contentEvents;

    public SiteSettingsResolver(
        IContentLoader contentLoader,
        ISynchronizedObjectInstanceCache cache,
        IContentEvents contentEvents)
    {
        _contentLoader = contentLoader;
        _cache = cache;
        _contentEvents = contentEvents;
        _contentEvents.PublishedContent += OnSiteSettingsChanged;
        _contentEvents.MovedContent += OnSiteSettingsChanged;
        _contentEvents.DeletedContent += OnSiteSettingsChanged;
    }

    public SiteSettingsPage? Get(SiteDefinition siteDefinition, CultureInfo culture)
    {
        // Settings belong to a site, but the cache key also includes language for future localized settings.
        var cacheKey = $"site-settings:{siteDefinition.Id}:{culture.Name}";
        if (_cache.Get(cacheKey) is SiteSettingsPage cachedSettings)
        {
            return cachedSettings;
        }

        var settings = _contentLoader
            .GetChildren<SiteSettingsPage>(siteDefinition.StartPage, new LanguageSelector(culture.Name))
            .SingleOrDefault();

        if (settings is not null)
        {
            _cache.Insert(
                cacheKey,
                settings,
                new CacheEvictionPolicy(
                    TimeSpan.FromMinutes(30),
                    CacheTimeoutType.Sliding,
                    cacheKeys: null,
                    masterKeys: [CacheMasterKey]));
        }

        return settings;
    }

    public void Dispose()
    {
        _contentEvents.PublishedContent -= OnSiteSettingsChanged;
        _contentEvents.MovedContent -= OnSiteSettingsChanged;
        _contentEvents.DeletedContent -= OnSiteSettingsChanged;
    }

    private void OnSiteSettingsChanged(object? sender, ContentEventArgs eventArgs)
    {
        if (eventArgs.Content is not SiteSettingsPage)
        {
            return;
        }

        // Removing the master key clears every site/language entry and propagates to other CMS instances.
        _cache.Remove(CacheMasterKey);
    }
}
