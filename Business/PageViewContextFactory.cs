using EPiServer.ServiceLocation;
using EPiServer.Web;
using TrainingTest.Business.Models;
using TrainingTest.Models.Pages;

namespace TrainingTest.Business;

[ServiceConfiguration]
public class PageViewContextFactory(IContentLoader contentLoader) 
{
    public virtual LayoutModel CreateLayoutModel(ContentReference currentContentLink)
    {
        var startPageContentLink = SiteDefinition.Current.StartPage;

        // Use the content link with version information when editing the startpage,
        // otherwise the published version will be used when rendering the props below.
        if (currentContentLink.CompareToIgnoreWorkID(startPageContentLink))
        {
            startPageContentLink = currentContentLink;
        }

        var siteSettings = contentLoader.GetChildren<SiteSettingsPage>(startPageContentLink).FirstOrDefault();

        return new LayoutModel
        {
            SiteName = siteSettings?.SiteName
        };
    }
}