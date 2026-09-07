using EPiServer.Find.Cms;
using TrainingTest.Models.Blocks;
using TrainingTest.Models.Pages;

namespace TrainingTest.Business.Authoring;

public interface IAuthorService
{
    Task<AuthorProfileBlock?> GetBySlug(string slug);
    Task<IContentResult<BlogPostPage>> GetPosts(BlogListPage blog, AuthorProfileBlock author, int pageNumber, int pageSize);
    Task<string?> GetUrl(BlogPostPage post);
    Task<Dictionary<string, string>> GetUrls(BlogListPage blog, List<string> authorNames);
    Task<string?> GetFirstAuthorUrl(BlogListPage blog);
    string? GetPartialUrl(BlogListPage blog, AuthorProfileBlock author, int pageNumber = 1);
}
