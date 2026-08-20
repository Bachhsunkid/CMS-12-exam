using EPiServer.Filters;
using TrainingTest.Models.Pages;
using TrainingTest.Models.ViewModels;

namespace TrainingTest.Business.Blog;

public class BlogPostQueryService(IContentLoader contentLoader) : IBlogPostQueryService
{
    public BlogListViewModel GetPosts(BlogListPage blog, int requestedPage)
    {
        var pageSize = Math.Clamp(blog.PageSize ?? Constants.DefaultPageSize, 1, 50);
        var visitorFilter = new FilterContentForVisitor();

        var allPosts = contentLoader
            .GetChildren<BlogPostPage>(blog.ContentLink) // Get all posts below this BlogListPage
            .Where(post => !visitorFilter.ShouldFilter(post)) // Filter out inaccessible content for this visitor
            .Where(post => post.PublishDate is null || post.PublishDate.Value <= DateTime.Now) // Filter out posts scheduled for the future.
            .OrderByDescending(post => post.PublishDate ?? DateTime.MinValue)
            .ThenByDescending(post => post.Changed)
            .ToList();

        var totalPages = Math.Max(1, (int)Math.Ceiling(allPosts.Count / (double)pageSize));
        var currentPage = requestedPage >= 1 && requestedPage <= totalPages
            ? requestedPage
            : 1;

        var posts = allPosts
            .Skip((currentPage - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new BlogListViewModel
        {
            Blog = blog,
            Posts = posts,
            CurrentPageNumber = currentPage,
            PageSize = pageSize,
            TotalPosts = allPosts.Count,
            TotalPages = totalPages
        };
    }
}
