using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using TrainingTest.Business.Models.Enums;

namespace TrainingTest.Business.Models;

public record SiteSearchRequest
{
    [FromQuery(Name = "q")]
    public string Query { get; init; } = string.Empty;
    
    [FromQuery(Name = "type")]
    public SiteSearchType Type { get; init; } = SiteSearchType.All;
    
    [Range(1, int.MaxValue)]
    [FromQuery(Name = "page")]
    public int Page { get; init; } = 1;
}
