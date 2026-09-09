namespace TrainingTest;

public static class Constants
{
    public const int DefaultPageSize = 6;
    public const int DefaultSearchPageSize = 8;
    public const int MaxPageSize = 100;
    public const int TagFacetSize = 50;

    public static readonly (int Days, string Label)[] Periods = [
        (7, "Last 7 days"),
        (30, "Last 30 days"),
        (90, "Last 90 days")
    ];
}
