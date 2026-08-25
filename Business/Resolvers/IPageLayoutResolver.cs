using TrainingTest.Business.Models;
using TrainingTest.Models.Pages;

namespace TrainingTest.Business.Resolvers;

public interface IPageLayoutResolver
{
    PageLayoutModel Resolve(SitePageData page);
}
