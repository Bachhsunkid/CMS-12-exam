using EPiServer.Find;
using EPiServer.Find.Cms;
using TrainingTest.Business.Authoring;
using TrainingTest.Business.Helpers;
using TrainingTest.Business.Models;
using TrainingTest.Models.Pages;
using TrainingTest.Models.ViewModels;

namespace TrainingTest.Business.Search;

/// <summary>
/// Queries the Search & Navigation index for the visitor-facing Blog List.
/// See https://docs.developers.optimizely.com/content-management-system/v1.1.0-search-and-navigation/docs/filters
/// https://docs.developers.optimizely.com/content-management-system/v1.1.0-search-and-navigation/docs/facets
/// and https://docs.developers.optimizely.com/content-management-system/v1.1.0-search-and-navigation/docs/pagination-skip-and-take
/// </summary>
public class BlogSearchService(
    IClient client,
    IAuthorService authorService,
    ILogger<BlogSearchService> logger) : IBlogSearchService
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(1);

    public async Task<BlogSearchViewModel> SearchAsync(BlogListPage blog, BlogSearchRequest request)
    {
        var pageSize = blog.PageSize ?? Constants.DefaultPageSize;
        var searchNow = SearchQueryCacheHelper.GetSearchNow();

        try
        {
            var postsTask = FetchPostsAsync(blog, request, searchNow, request.Page, pageSize);
            var facetsTask = FetchFacetsAsync(blog, request, searchNow);

            await Task.WhenAll(postsTask, facetsTask);
            var posts = await postsTask;
            var facets = await facetsTask;

            return await MapToViewModel(blog, request, request.Page, pageSize, posts, facets);
        }
        catch (Exception ex)
        {
            // show a warning in the UI to indicate that search is unavailable
            logger.LogWarning(ex, "Blog search unavailable for blog {BlogId}.", blog.ContentLink.ID);
            return BlogSearchViewModel.CreateUnavailableResult(blog, request);
        }
    }

    private Task<IContentResult<BlogPostPage>> FetchPostsAsync(
        BlogListPage blog, BlogSearchRequest request, DateTime searchNow, int page, int pageSize)
    {
        var query = client.Search<BlogPostPage>();

        // var test = BaseQuery(query, blog, request, searchNow)
        //     .WithTag(request.Tag)
        //     .WithPeriod(request.PeriodDays, searchNow)
        //     .WithSort(request.Sort)
        //     .TermsFacetFor(p => p.Tags, command => command.Size = Constants.TagFacetSize)
        //     .FilterFacet(Constants.Periods[0].Label, p => p.PublishDate.InRange(searchNow.AddDays(-Constants.Periods[0].Days), searchNow))
        //     .FilterFacet(Constants.Periods[1].Label, p => p.PublishDate.InRange(searchNow.AddDays(-Constants.Periods[1].Days), searchNow))
        //     .FilterFacet(Constants.Periods[2].Label, p => p.PublishDate.InRange(searchNow.AddDays(-Constants.Periods[2].Days), searchNow))
        //     .FilterFacet("all", p => p.PublishDate.Before(searchNow))
        //     .Skip((page - 1) * pageSize)
        //     .Take(pageSize) // default is 10, maximum is 1000
        //     .GetContentResultAsync().Result;

        return BaseQuery(query, blog, request, searchNow)
            .WithTag(request.Tag)
            .WithPeriod(request.PeriodDays, searchNow)
            .WithSort(request.Sort)
            .Skip((page - 1) * pageSize)
            .Take(pageSize) // default is 10, maximum is 1000
            .GetContentResultAsync();
    }

    private async Task<BlogFacetResults> FetchFacetsAsync(
        BlogListPage blog, BlogSearchRequest request, DateTime searchNow)
    {
        var results = await client.MultiSearch<BlogPostFacetSearchHit>()
            .Search<BlogPostPage, BlogPostFacetSearchHit>(q =>
                BaseQuery(q, blog, request, searchNow)
                    .WithPeriod(request.PeriodDays, searchNow)
                    .TermsFacetFor(p => p.Tags, command => command.Size = Constants.TagFacetSize)
                    .Take(0)
                    .StaticallyCacheFor(CacheDuration)
                    .Select(p => new BlogPostFacetSearchHit { Tags = p.Tags }))
            .Search<BlogPostPage, BlogPostFacetSearchHit>(q =>
                AddPeriodFacets(BaseQuery(q, blog, request, searchNow).WithTag(request.Tag), searchNow)
                    .Take(0)
                    .StaticallyCacheFor(CacheDuration)
                    .Select(p => new BlogPostFacetSearchHit { Tags = p.Tags }))
            .GetResultAsync();

        var resultList = results.ToList();
        return new BlogFacetResults(
            BuildTagFacets(resultList[0], request.Tag),
            BuildPeriodFacets(resultList[1], request.PeriodDays));
    }

    private static ITypeSearch<BlogPostPage> BaseQuery(ITypeSearch<BlogPostPage> query, BlogListPage blog,
        BlogSearchRequest request, DateTime searchNow)
    {
        if (!string.IsNullOrWhiteSpace(request.Query))
        {
            query = query.For(request.Query)
                .InFields(p => p.Title, p => p.Summary, p => p.GetSearchableMainBody())
                .BoostMatching(p => p.PublishDate.InRange(searchNow.AddDays(-90), searchNow), 1.5)
                .BoostMatching(p => p.Tags.Match("optimizely"), 2)
                .ApplyBestBets();
        }

        query = query.FilterOnCurrentSite()
            .FilterForVisitor()
            .Filter(p => p.ParentLink.ID.Match(blog.ContentLink.ID))
            .Filter(p => p.PublishDate.Before(searchNow));

        if (request.QuickReadsOnly)
        {
            query = query.Filter(p => p.GetReadingTimeMinutes().LessThan(3));
        }
        
        return query;
    }

    // Adds a facet filter for each period, so that the facet counts are correct when a tag is selected.
    private static ITypeSearch<BlogPostPage> AddPeriodFacets(ITypeSearch<BlogPostPage> query, DateTime searchNow)
    {
        foreach (var period in Constants.Periods)
        {
            query = query.FilterFacet(PeriodFacetKey(period.Days), p =>
                p.PublishDate.InRange(searchNow.AddDays(-period.Days), searchNow));
        }
        return query;
    }

    private static string PeriodFacetKey(int days) => $"period-{days}";

    private async Task<BlogSearchViewModel> MapToViewModel(
        BlogListPage blog, BlogSearchRequest request, int page, int pageSize,
        IContentResult<BlogPostPage> posts, BlogFacetResults facets)
    {
        var authorNames = posts
            .Select(p => p.Author)
            .Where(a => !string.IsNullOrWhiteSpace(a)).Distinct()
            .ToList();

        var authorUrls = await authorService.GetUrls(blog, authorNames!);

        return new(blog)
        {
            Request = request,
            TagFacets = facets.Tags,
            PeriodFacets = facets.Periods,
            Posts = posts.Select(post => new BlogPostListItemViewModel
            {
                Post = post,
                AuthorUrl = authorUrls.TryGetValue(post.Author ?? string.Empty, out var url) ? url : null
            }).ToList(),
            Paging = new PagingViewModelBase
            {
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = posts.TotalMatching
            }
        };
    }

    private static IReadOnlyList<BlogSearchFacetOption> BuildTagFacets(SearchResults<BlogPostFacetSearchHit> result, string? selectedTag)
    {
        return result
            .TermsFacetFor(p => p.Tags).Terms
            .OrderBy(facet => facet.Term)
            .Select(facet => new BlogSearchFacetOption
            {
                Value = facet.Term,
                Label = facet.Term,
                Count = facet.Count,
                IsSelected = string.Equals(selectedTag, facet.Term, StringComparison.OrdinalIgnoreCase)
            }).ToList();
    }

    private static IReadOnlyList<BlogSearchFacetOption> BuildPeriodFacets(
        SearchResults<BlogPostFacetSearchHit> result,
        int? selectedPeriodDays)
    {
        var options = Constants.Periods
            .Select(period => new BlogSearchFacetOption
            {
                Value = period.Days.ToString(),
                Label = period.Label,
                Count = result.FilterFacet(PeriodFacetKey(period.Days)).Count,
                IsSelected = selectedPeriodDays == period.Days
            }).ToList();
        options.Add(new BlogSearchFacetOption
        {
            Value = "all", Label = "All time", Count = result.TotalMatching,
            IsSelected = !selectedPeriodDays.HasValue
        });
        return options;
    }

    private record BlogFacetResults(
        IReadOnlyList<BlogSearchFacetOption> Tags,
        IReadOnlyList<BlogSearchFacetOption> Periods);
}
