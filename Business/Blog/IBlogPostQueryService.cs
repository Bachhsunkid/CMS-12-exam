using TrainingTest.Models.Pages;
using TrainingTest.Models.ViewModels;

namespace TrainingTest.Business.Blog;

public interface IBlogPostQueryService
{
    BlogListViewModel GetPosts(BlogListPage blog, int requestedPage);
}
