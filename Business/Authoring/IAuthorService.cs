using EPiServer.Find.Cms;
using TrainingTest.Models.Blocks;
using TrainingTest.Models.Pages;

namespace TrainingTest.Business.Authoring;

public interface IAuthorService
{
    Task<AuthorProfileBlock?> GetBySlug(string slug);
    AuthorProfileBlock? GetByReference(ContentReference? authorReference);
    Task<IContentResult<BlogPostPage>> GetPosts(BlogListPage blog, AuthorProfileBlock author, int pageNumber, int pageSize);
    string? GetUrl(BlogPostPage post);
    Dictionary<int, AuthorReferenceDetails> GetUrls(BlogListPage blog, IEnumerable<ContentReference> authorReferences);
    Task<string?> GetFirstAuthorUrl(BlogListPage blog);
    string? GetPartialUrl(BlogListPage blog, AuthorProfileBlock author, int pageNumber = 1);
}
