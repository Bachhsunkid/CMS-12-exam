using EPiServer.Web.Routing;
using Microsoft.AspNetCore.WebUtilities;
using TrainingTest.Business.Models.Enums;
using TrainingTest.Models.ViewModels;

namespace TrainingTest.Business.Search;

public class SearchUrlBuilder(IUrlResolver urlResolver) : ISearchUrlBuilder
{
    public string GetBlogListSearchUrl(BlogSearchViewModel model, string? tag, int? periodDays, int page = 1)
    {
        var query = new Dictionary<string, string?>();

        if (!string.IsNullOrWhiteSpace(model.Request.Query)) query["q"] = model.Request.Query;
        if (!string.IsNullOrWhiteSpace(tag)) query["tag"] = tag;
        if (periodDays.HasValue) query["period"] = periodDays.Value.ToString();
        if (model.Request.QuickReadsOnly) query["quickreads"] = "true";
        if (model.Request.Sort == BlogSearchSort.Newest) query["sort"] = "newest";
        if (page > 1) query["page"] = page.ToString();

        var blogListUrl = urlResolver.GetUrl(model.Blog.ContentLink);
        return QueryHelpers.AddQueryString(blogListUrl, query);
    }

    public string GetSiteSearchUrl(SiteSearchViewModel model, SiteSearchType type, int page = 1)
    {
        var query = new Dictionary<string, string?>();

        if (!string.IsNullOrWhiteSpace(model.Request.Query)) query["q"] = model.Request.Query;
        if (type != SiteSearchType.All) query["type"] = type.ToString();
        if (page > 1) query["page"] = page.ToString();

        var siteSearchUrl = urlResolver.GetUrl(model.Page.ContentLink);
        return QueryHelpers.AddQueryString(siteSearchUrl, query);
    }

    public string GetSiteSearchTypeLabel(SiteSearchType type) => type switch
    {
        SiteSearchType.All => "All",
        SiteSearchType.Page => "Pages",
        SiteSearchType.BlogPost => "Blog posts",
        SiteSearchType.Document => "Documents",
        _ => "Unknown type"
    };
}
