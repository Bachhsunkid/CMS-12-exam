using TrainingTest.Models.Pages;

namespace TrainingTest.Business.Authoring;

public sealed record AuthorPostSearchResult(IReadOnlyList<BlogPostPage> Posts, int TotalPosts);
