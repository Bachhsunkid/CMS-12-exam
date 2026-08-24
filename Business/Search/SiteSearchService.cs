using EPiServer.Find;
using EPiServer.Find.Cms;
using EPiServer.Find.UnifiedSearch;
using EPiServer.Web.Routing;
using TrainingTest.Business.Helpers;
using TrainingTest.Business.Models;
using TrainingTest.Business.Models.Enums;
using TrainingTest.Models.Media;
using TrainingTest.Models.Pages;
using TrainingTest.Models.ViewModels;

namespace TrainingTest.Business.Search;

/// <summary>
/// Searches the public site across pages, blog posts and document media in one result list.
/// See https://docs.developers.optimizely.com/content-management-system/v1.1.0-search-and-navigation/docs/unified-search
/// and https://docs.developers.optimizely.com/content-management-system/v1.1.0-search-and-navigation/docs/boosting-with-weights
/// </summary>
public class SiteSearchService(IClient client, UrlResolver urlResolver) : ISiteSearchService
{
    private readonly HitSpecification _hitSpec = new HitSpecification
    {
        HighlightExcerpt = true // Highlight the excerpt in the search result
    };

    public async Task<SiteSearchViewModel> SearchAsync(SiteSearchPage searchPage, SiteSearchRequest request)
    {
        ITypeSearch<ISearchContent> search = string.IsNullOrWhiteSpace(request.Query)
            ? client.UnifiedSearch()
            : client.UnifiedSearch().For(request.Query);

        search = ApplyTypeFilter(search, request.Type);

        var searchResult = await search
            .Skip(PaginationHelper.GetSkip(request.Page, Constants.DefaultSearchPageSize))
            .Take(Constants.DefaultSearchPageSize)
            .GetResultAsync(_hitSpec);

        var totalPages = PaginationHelper.GetTotalPages(searchResult.TotalMatching, Constants.DefaultSearchPageSize);
        var currentPage = PaginationHelper.NormalizePage(request.Page, totalPages);

        if (currentPage != request.Page)
        {
            searchResult = await search
                .Skip(PaginationHelper.GetSkip(currentPage, Constants.DefaultSearchPageSize))
                .Take(Constants.DefaultSearchPageSize)
                .GetResultAsync(_hitSpec);
        }

        return new SiteSearchViewModel
        {
            Page = searchPage,
            Request = request,
            Results = searchResult.Select(MapResult).ToList(),
            TotalResults = searchResult.TotalMatching,
            CurrentPageNumber = currentPage,
            TotalPages = totalPages
        };
    }

    private static ITypeSearch<ISearchContent> ApplyTypeFilter(
        ITypeSearch<ISearchContent> search,
        SiteSearchType type)
    {
        return type switch
        {
            SiteSearchType.Page => search.Filter(content => content.MatchTypeHierarchy(typeof(PageData)) & !content.MatchTypeHierarchy(typeof(BlogPostPage))),
            SiteSearchType.BlogPost => search.Filter(content => content.MatchTypeHierarchy(typeof(BlogPostPage))),
            SiteSearchType.Document => search.Filter(content => content.MatchTypeHierarchy(typeof(DocumentFile))),
            _ => search
        };
    }

    private SiteSearchResultItem MapResult(UnifiedSearchHit hit)
    {
        var content = hit.OriginalObjectGetter?.Invoke() as IContent;
        var type = GetResultType(hit.OriginalObjectType);
        var excerpt = hit.Excerpt;

        return new SiteSearchResultItem
        {
            Type = type,
            Title = hit.Title,
            Excerpt = string.IsNullOrWhiteSpace(excerpt) ? GetFallbackExcerpt(content) : excerpt,
            Url = content is null ? hit.Url : urlResolver.GetUrl(content.ContentLink),
            HasHighlightedExcerpt = !string.IsNullOrWhiteSpace(excerpt)
        };
    }

    private static SiteSearchType GetResultType(Type? originalObjectType)
    {
        if (originalObjectType is not null && typeof(DocumentFile).IsAssignableFrom(originalObjectType))
        {
            return SiteSearchType.Document;
        }

        if (originalObjectType is not null && typeof(BlogPostPage).IsAssignableFrom(originalObjectType))
        {
            return SiteSearchType.BlogPost;
        }
        
        if(originalObjectType is not null && typeof(PageData).IsAssignableFrom(originalObjectType))
        {
            return SiteSearchType.Page;
        }

        return SiteSearchType.Unknown;
    }

    private static string GetFallbackExcerpt(IContent? content) => content switch
    {
        BlogPostPage post => post.Summary ?? post.GetSearchableMainBody(),
        SitePageData page => page.MetaDescription ?? string.Empty,
        _ => string.Empty
    };
}
