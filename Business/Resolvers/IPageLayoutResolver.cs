using TrainingTest.Business.Models;
using TrainingTest.Models.Blocks;
using TrainingTest.Models.Pages;

namespace TrainingTest.Business.Resolvers;

public interface IPageLayoutResolver
{
    LayoutModel Resolve(SitePageData page);
    HeaderBlock? ResolveHeader(SitePageData page);
    FooterBlock? ResolveFooter(SitePageData page);
}
