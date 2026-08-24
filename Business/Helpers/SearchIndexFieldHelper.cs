using System.Net;
using System.Text.RegularExpressions;
using TrainingTest.Models.Pages;

namespace TrainingTest.Business.Helpers;

public static class SearchIndexFieldHelper
{
    public static int GetReadingTimeMinutes(this BlogPostPage post)
    {
        // Find uses this computed value for the quick-read filter in Task 7.
        return Math.Max(1, post.MainBody.EstimateReadingTime());
    }

    public static int GetAgeInDays(this BlogPostPage post)
    {
        // Uses the same local CMS time convention as publishing and seed data.
        if (post.PublishDate is null)
        {
            return 0;
        }

        return Math.Max(0, (int)Math.Floor((DateTime.Now - post.PublishDate.Value).TotalDays));
    }
}
