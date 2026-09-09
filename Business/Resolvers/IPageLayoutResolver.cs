using TrainingTest.Models.Blocks;
using TrainingTest.Models.Pages;

namespace TrainingTest.Business.Resolvers;

public interface IPageLayoutResolver
{
    HeaderBlock? ResolveHeader(SitePageData page);
    FooterBlock? ResolveFooter(SitePageData page);
}
