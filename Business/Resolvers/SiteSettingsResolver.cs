using System.Collections.Concurrent;
using System.Globalization;
using EPiServer.Web;
using Microsoft.Extensions.Caching.Memory;
using TrainingTest.Models.Pages;

namespace TrainingTest.Business.Resolvers;

public class SiteSettingsResolver : ISiteSettingsResolver, IDisposable
{
    // Finds the shared SiteSettingsPage for the current site, so pages do not load settings themselves.
    // Reference: https://docs.developers.optimizely.com/content-management-system/docs/object-caching
    private readonly IContentLoader _contentLoader;
    private readonly IMemoryCache _cache;
    private readonly IContentEvents _contentEvents;

    // Content events do not identify every site/language cache entry that may reference the changed settings.
    private readonly ConcurrentDictionary<string, byte> _cacheKeys = new();

    public SiteSettingsResolver(IContentLoader contentLoader, IMemoryCache cache, IContentEvents contentEvents)
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
        _cacheKeys.TryAdd(cacheKey, 0);

        return _cache.GetOrCreate(cacheKey, entry =>
        {
            entry.SlidingExpiration = TimeSpan.FromMinutes(30);
            return _contentLoader
                .GetChildren<SiteSettingsPage>(siteDefinition.StartPage, new LanguageSelector(culture.Name))
                .SingleOrDefault();
        });
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

        foreach (var cacheKey in _cacheKeys.Keys)
        {
            _cache.Remove(cacheKey);
        }

        // A settings publish/move/delete can change the result for any site/language lookup.
        _cacheKeys.Clear();
    }
}
