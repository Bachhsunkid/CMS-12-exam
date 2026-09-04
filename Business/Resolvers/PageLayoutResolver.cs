using System.Globalization;
using EPiServer.Web;
using TrainingTest.Business.Models;
using TrainingTest.Models.Blocks;
using TrainingTest.Models.Pages;

namespace TrainingTest.Business.Resolvers;

public class PageLayoutResolver(
    ISiteSettingsResolver siteSettingsResolver,
    IContentLoader contentLoader) : IPageLayoutResolver
{
    // Resolves the header and footer rendered for a page from its Inherit, Override, or Hidden behavior.
    // Reference: https://docs.developers.optimizely.com/content-management-system/docs/dependency-injection
    public LayoutModel Resolve(SitePageData page)
    {
        var settings = siteSettingsResolver.Get(SiteDefinition.Current, CultureInfo.CurrentUICulture);

        return new LayoutModel
        {
            Header = ResolveBlock<HeaderBlock>((HeaderFooterBehavior)page.HeaderBehavior, page.HeaderOverride, settings?.DefaultHeader),
            Footer = ResolveBlock<FooterBlock>((HeaderFooterBehavior)page.FooterBehavior, page.FooterOverride, settings?.DefaultFooter)
        };
    }
    
    public HeaderBlock? ResolveHeader(SitePageData page)
    {
        var settings = siteSettingsResolver.Get(SiteDefinition.Current, CultureInfo.CurrentUICulture);
        return ResolveBlock<HeaderBlock>((HeaderFooterBehavior)page.HeaderBehavior, page.HeaderOverride, settings?.DefaultHeader);
    }
    
    public FooterBlock? ResolveFooter(SitePageData page)
    {
        var settings = siteSettingsResolver.Get(SiteDefinition.Current, CultureInfo.CurrentUICulture);
        return ResolveBlock<FooterBlock>((HeaderFooterBehavior)page.FooterBehavior, page.FooterOverride, settings?.DefaultFooter);
    }

    private TBlock? ResolveBlock<TBlock>(
        HeaderFooterBehavior behavior,
        ContentReference? pageOverride,
        ContentReference? siteDefault)
        where TBlock : BlockData
    {
        // Hidden must return no block so the layout emits no empty header/footer markup.
        // Override takes precedence over the site default;
        // Inherit falls through to the default.
        var contentLink = behavior switch
        {
            HeaderFooterBehavior.Hidden => null,
            HeaderFooterBehavior.Override => pageOverride,
            _ => siteDefault
        };

        return ContentReference.IsNullOrEmpty(contentLink)
            ? null
            : contentLoader.TryGet(contentLink, out TBlock? block)
                ? block
                : null;
    }
}
