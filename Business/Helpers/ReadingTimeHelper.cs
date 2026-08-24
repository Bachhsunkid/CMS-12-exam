using System.Text.RegularExpressions;

namespace TrainingTest.Business.Helpers;

public static class ReadingTimeHelper
{
    private const int WordsPerMinute = 200;

    public static int EstimateReadingTime(this XhtmlString? xhtmlString)
    {
        if (xhtmlString is null || xhtmlString.IsEmpty)
        {
            return 0;
        }

        var plainText = Regex.Replace(xhtmlString.ToHtmlString(), "<[^>]+>", " ");
        var words = Regex.Matches(plainText, @"\b\w+\b").Count;

        return Math.Max(1, (int)Math.Ceiling((double)words / WordsPerMinute));
    }

    public static int EstimateReadingTime(this string? plainText)
    {
        if (string.IsNullOrWhiteSpace(plainText))
        {
            return 0;
        }

        var words = Regex.Matches(plainText, @"\b\w+\b").Count;
        return Math.Max(1, (int)Math.Ceiling((double)words / WordsPerMinute));
    }
}
