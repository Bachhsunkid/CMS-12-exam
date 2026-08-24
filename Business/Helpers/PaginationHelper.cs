namespace TrainingTest.Business.Helpers;

/// <summary>
/// Keeps page-number, offset and total-page calculations consistent across server-rendered listings.
/// </summary>
public static class PaginationHelper
{
    public static int GetTotalPages(int totalItems, int pageSize)
    {
        return Math.Max(1, (int)Math.Ceiling(totalItems / (double)pageSize));
    }

    public static int NormalizePage(int requestedPage, int totalPages)
    {
        return requestedPage >= 1 && requestedPage <= totalPages 
            ? requestedPage 
            : 1;
    }

    public static int GetSkip(int pageNumber, int pageSize)
    {
        return (Math.Max(1, pageNumber) - 1) * pageSize;
    }
}
