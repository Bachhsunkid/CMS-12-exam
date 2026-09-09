namespace TrainingTest.Models.ViewModels;

/// <summary>
/// Pagination metadata shared by server-rendered result listings.
/// </summary>
public class PagingViewModelBase
{
    public required int CurrentPage { get; init; }
    public required int PageSize { get; init; }
    public required int TotalItems { get; init; }
    public int TotalPages => Math.Max(1, (int)Math.Ceiling(TotalItems / (double)PageSize));
    public bool HasPrev => CurrentPage > 1;
    public bool HasNext => CurrentPage < TotalPages;
}
