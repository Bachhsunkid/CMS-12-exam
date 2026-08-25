using TrainingTest.Business.Models;
using TrainingTest.Models.Pages;
using TrainingTest.Models.ViewModels;

namespace TrainingTest.Business.Search;

public interface IBlogSearchService
{
    Task<BlogSearchViewModel> SearchAsync(BlogListPage blog, BlogSearchRequest request);
}
