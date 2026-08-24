namespace TrainingTest;

public static class Constants
{
    public const int DefaultPageSize = 6;

    public static readonly (int Days, string Label)[] Periods = [
        (7, "Last 7 days"),
        (30, "Last 30 days"),
        (90, "Last 90 days")
    ];
}