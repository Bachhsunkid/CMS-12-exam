using TrainingTest.Models.Blocks;
using TrainingTest.Models.Pages;

namespace TrainingTest.Business.Authoring;

public interface IAuthorService
{
    AuthorProfileBlock? GetBySlug(string slug);
    IReadOnlyList<BlogPostPage> GetPosts(BlogListPage blog, AuthorProfileBlock author);
    string? GetUrl(BlogPostPage post);
    string? GetUrl(BlogListPage blog, string? authorName);
    string? GetFirstAuthorUrl(BlogListPage blog);
    string GetUrl(BlogListPage blog, AuthorProfileBlock author, int pageNumber = 1);
}
