namespace TrainingTest.Business.Search;

/// <summary>
/// Projection type for facet-only multi-search queries.
/// Facet counts are read from the result aggregations; <c>Take(0)</c> means
/// instances of this type are never returned.
/// </summary>
internal sealed class BlogPostFacetSearchHit
{
    public IList<string>? Tags { get; init; }
}
