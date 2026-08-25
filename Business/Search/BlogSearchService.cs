using EPiServer.Find;
using EPiServer.Find.Cms;
using TrainingTest.Business.Helpers;
using TrainingTest.Business.Models;
using TrainingTest.Business.Models.Enums;
using TrainingTest.Business.Authoring;
using TrainingTest.Models.Pages;
using TrainingTest.Models.ViewModels;

namespace TrainingTest.Business.Search;

/// <summary>
/// Queries the Search & Navigation index for the visitor-facing Blog List.
/// See https://docs.developers.optimizely.com/content-management-system/v1.1.0-search-and-navigation/docs/filters
/// https://docs.developers.optimizely.com/content-management-system/v1.1.0-search-and-navigation/docs/facets
/// and https://docs.developers.optimizely.com/content-management-system/v1.1.0-search-and-navigation/docs/pagination-skip-and-take
/// </summary>
public class BlogSearchService(IClient client, IAuthorService authorService) : IBlogSearchService
{
    public async Task<BlogSearchViewModel> SearchAsync(BlogListPage blog, BlogSearchRequest request)
    {
        var pageSize = blog.PageSize ?? Constants.DefaultPageSize;
        var blogSearchClient = client.Search<BlogPostPage>();

        var selectedPostsSearch = ApplySelectedFilters(blogSearchClient, blog, request, includeTag: true, includePeriod: true);
        if (request.Sort == BlogSearchSort.Newest)
        {
            selectedPostsSearch = selectedPostsSearch
                .OrderByDescending(post => post.PublishDate)
                .ThenByDescending(post => post.Changed);
        }

        var selectedPostsTask = selectedPostsSearch
            .Skip(PaginationHelper.GetSkip(request.Page, pageSize))
            .Take(pageSize)
            .GetContentResultAsync();

        // A tag facet must retain every active restriction except the tag itself, otherwise
        // selecting one tag would hide the useful counts for the other tags.
        var tagFacetTask = ApplySelectedFilters(blogSearchClient, blog, request, includeTag: false, includePeriod: true)
            .TermsFacetFor(post => post.Tags, options => options.Size = 100)
            .Take(0)
            .GetContentResultAsync();

        // The same rule applies independently to the publish-date facet.
        var periodFacetTask = AddPeriodFacets(
                ApplySelectedFilters(blogSearchClient, blog, request, includeTag: true, includePeriod: false))
            .Take(0)
            .GetContentResultAsync();

        await Task.WhenAll(selectedPostsTask, tagFacetTask, periodFacetTask);

        var selectedPostsResult = await selectedPostsTask;
        var tagFacetSearchResult = await tagFacetTask;
        var periodFacetSearchResult = await periodFacetTask;

        var totalPages = PaginationHelper.GetTotalPages(selectedPostsResult.TotalMatching, pageSize);
        var currentPage = PaginationHelper.NormalizePage(request.Page, totalPages);

        if (currentPage != request.Page)
        {
            selectedPostsResult = await ApplySelectedFilters(blogSearchClient, blog, request, includeTag: true, includePeriod: true)
                .Skip(PaginationHelper.GetSkip(currentPage, pageSize))
                .Take(pageSize)
                .GetContentResultAsync();
        }

        return new BlogSearchViewModel
        {
            Blog = blog,
            Request = request,
            Posts = selectedPostsResult.Items
                .Select(post => new BlogPostListItemViewModel
                {
                    Post = post,
                    AuthorUrl = authorService.GetUrl(blog, post.Author)
                })
                .ToList(),
            TagFacets = tagFacetSearchResult
                .TermsFacetFor(post => post.Tags)
                .Terms
                .OrderByDescending(facet => facet.Count)
                .ThenBy(facet => facet.Term)
                .Select(facet => new BlogSearchFacetOption
                {
                    Value = facet.Term,
                    Label = facet.Term,
                    Count = facet.Count,
                    IsSelected = string.Equals(request.Tag, facet.Term, StringComparison.OrdinalIgnoreCase)
                })
                .ToList(),
            PeriodFacets = CreatePeriodFacets(periodFacetSearchResult, request.PeriodDays),
            CurrentPageNumber = currentPage,
            PageSize = pageSize,
            TotalPosts = selectedPostsResult.TotalMatching,
            TotalPages = totalPages
        };
    }

    private static ITypeSearch<BlogPostPage> ApplySelectedFilters(
        ITypeSearch<BlogPostPage> query,
        BlogListPage blog,
        BlogSearchRequest request,
        bool includeTag,
        bool includePeriod)
    {
        if (!string.IsNullOrWhiteSpace(request.Query))
        {
            query = query.For(request.Query)
                .InFields(
                    post => post.Title,
                    post => post.Summary,
                    post => post.GetSearchableMainBody())
                .BoostMatching(post => post.GetAgeInDays().LessThan(91), 1000)
                .BoostMatching(post => post.Tags.Match("optimizely"), 2000)
                .ApplyBestBets();
        }
        
        // Find requires For(...) before any filter. These CMS filters then keep every Blog List
        // query within the site, language, visitor access and direct children of this Blog List.
        query = query
            .FilterOnCurrentSite()
            .FilterForVisitor()
            .Filter(post => post.ParentLink.ID.Match(blog.ContentLink.ID))
            .Filter(post => post.PublishDate.InRange(DateTime.MinValue, DateTime.Now));

        if (includeTag && request.Tag is not null)
        {
            query = query.Filter(post => post.Tags.Match(request.Tag));
        }

        if (includePeriod && request.PeriodDays.HasValue)
        {
            query = query
                .Filter(post => post.PublishDate.InRange(DateTime.Now.AddDays(-request.PeriodDays.Value), DateTime.Now));
        }

        if (request.QuickReadsOnly)
        {
            query = query.Filter(post => post.GetReadingTimeMinutes().LessThan(4));
        }

        return query;
    }

    private static ITypeSearch<BlogPostPage> AddPeriodFacets(ITypeSearch<BlogPostPage> query)
    {
        foreach (var period in Constants.Periods)
        {
            var from = DateTime.Now.AddDays(-period.Days);
            query = query.FilterFacet($"period-{period.Days}", post => post.PublishDate.InRange(from, DateTime.Now));
        }

        return query;
    }

    private static IReadOnlyList<BlogSearchFacetOption> CreatePeriodFacets(
        IContentResult<BlogPostPage> result,
        int? selectedPeriodDays)
    {
        var facets = Constants.Periods
            .Select(period => new BlogSearchFacetOption
            {
                Value = period.Days.ToString(),
                Label = period.Label,
                Count = result.FilterFacet($"period-{period.Days}").Count,
                IsSelected = selectedPeriodDays == period.Days
            })
            .ToList();

        facets.Add(new BlogSearchFacetOption
        {
            Value = "all",
            Label = "All time",
            Count = result.TotalMatching,
            IsSelected = !selectedPeriodDays.HasValue
        });

        return facets;
    }
}
