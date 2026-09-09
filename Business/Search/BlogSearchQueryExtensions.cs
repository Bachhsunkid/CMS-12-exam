using EPiServer.Find;
using TrainingTest.Business.Models.Enums;
using TrainingTest.Models.Pages;

namespace TrainingTest.Business.Search;

public static class BlogSearchQueryExtensions
{
    public static ITypeSearch<BlogPostPage> WithTag(this ITypeSearch<BlogPostPage> query, string? tag)
    {
        return string.IsNullOrWhiteSpace(tag) ? query : query.Filter(post => post.Tags.Match(tag));
    }

    public static ITypeSearch<BlogPostPage> WithPeriod(this ITypeSearch<BlogPostPage> query, int? periodDays, DateTime searchNow)
    {
        return periodDays.HasValue
            ? query.Filter(post => post.PublishDate.InRange(searchNow.AddDays(-periodDays.Value), searchNow))
            : query;
    }

    public static ITypeSearch<BlogPostPage> WithSort(this ITypeSearch<BlogPostPage> query, BlogSearchSort sort)
    {
        return sort == BlogSearchSort.Newest
            ? query.OrderByDescending(post => post.PublishDate).ThenByDescending(post => post.Changed)
            : query;
    }
}
