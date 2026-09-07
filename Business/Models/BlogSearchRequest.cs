using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using TrainingTest.Business.Models.Enums;

namespace TrainingTest.Business.Models;

/// <summary>
/// Represents the Blog List query-string state.
/// </summary>
public class BlogSearchRequest
{
    [FromQuery(Name = "q")]
    public string Query { get; set; } = string.Empty;

    [FromQuery(Name = "tag")]
    public string? Tag { get; set; }

    [FromQuery(Name = "period")]
    public int? PeriodDays { get; set; }

    [FromQuery(Name = "quickreads")]
    public bool QuickReadsOnly { get; set; }

    [FromQuery(Name = "sort")]
    public BlogSearchSort Sort { get; set; } = BlogSearchSort.Relevance;

    [FromQuery(Name = "page")]
    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;
}
