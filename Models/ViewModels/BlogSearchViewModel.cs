using TrainingTest.Business.Models;
using TrainingTest.Models.Pages;

namespace TrainingTest.Models.ViewModels;

public class BlogSearchViewModel : PageViewModel<BlogListPage>
{
    public BlogSearchViewModel(BlogListPage currentPage)
        : base(currentPage)
    {
        Blog = currentPage;
    }

    public BlogListPage Blog { get; init; }
    public required BlogSearchRequest Request { get; init; }
    public required IReadOnlyList<BlogPostListItemViewModel> Posts { get; init; }
    public required IReadOnlyList<BlogSearchFacetOption> TagFacets { get; init; }
    public required IReadOnlyList<BlogSearchFacetOption> PeriodFacets { get; init; }
    public required PagingViewModelBase Paging { get; init; }
    public bool SearchUnavailable { get; init; }

    public static BlogSearchViewModel CreateUnavailableResult(BlogListPage blog, BlogSearchRequest request)
    {
        return new BlogSearchViewModel(blog)
        {
            Request = request,
            Posts = [],
            TagFacets = [],
            PeriodFacets = [],
            Paging = new PagingViewModelBase
            {
                CurrentPage = 1,
                PageSize = Constants.DefaultPageSize,
                TotalItems = 0
            },
            SearchUnavailable = true
        };
    }
}
