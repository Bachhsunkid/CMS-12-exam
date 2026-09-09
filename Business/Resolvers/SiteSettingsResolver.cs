using System.Globalization;
using EPiServer.Web;
using TrainingTest.Models.Pages;

namespace TrainingTest.Business.Resolvers;

public class SiteSettingsResolver(IContentLoader contentLoader) : ISiteSettingsResolver
{
    public SiteSettingsPage? Get(SiteDefinition siteDefinition, CultureInfo culture)
    {
        var settings = contentLoader
            .GetChildren<SiteSettingsPage>(siteDefinition.StartPage, new LanguageSelector(culture.Name))
            .SingleOrDefault();

        return settings;
    }
}
