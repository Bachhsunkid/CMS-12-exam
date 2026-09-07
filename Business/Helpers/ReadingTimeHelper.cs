using System.Text.RegularExpressions;

namespace TrainingTest.Business.Helpers;

public static class ReadingTimeHelper
{
    private const int WordsPerMinute = 200;
    private const string WordPattern = @"\b\w+\b";
    private const string HtmlTagPattern = @"<[^>]+>";

    public static int EstimateReadingTime(this XhtmlString? xhtmlString)
    {
        if (xhtmlString is null || xhtmlString.IsEmpty)
        {
            return 0;
        }

        var plainText = Regex.Replace(xhtmlString.ToHtmlString(), HtmlTagPattern, " ");
        var words = Regex.Matches(plainText, WordPattern).Count;

        return Math.Max(1, (int)Math.Ceiling((double)words / WordsPerMinute));
    }
}
