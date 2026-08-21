using System.Globalization;
using EPiServer.Web;
using TrainingTest.Models.Pages;

namespace TrainingTest.Business.Resolvers;

public interface ISiteSettingsResolver
{
    SiteSettingsPage? Get(SiteDefinition siteDefinition, CultureInfo culture);
}
