using TrainingTest.Models.Blocks;
using TrainingTest.Models.Pages;

namespace TrainingTest.Business.Authoring;

public interface IAuthorService
{
    AuthorProfileBlock? GetBySlug(string slug);
    Task<AuthorPostSearchResult> GetPosts(BlogListPage blog, AuthorProfileBlock author, int pageNumber, int pageSize);
    string? GetUrl(BlogPostPage post);
    string? GetUrl(BlogListPage blog, string? authorName);
    string GetUrl(BlogListPage blog, AuthorProfileBlock author, int pageNumber = 1);
    string? GetFirstAuthorUrl(BlogListPage blog);
}
