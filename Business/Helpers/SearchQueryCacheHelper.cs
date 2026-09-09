namespace TrainingTest.Business.Helpers;

public static class SearchQueryCacheHelper
{
    public static DateTime GetSearchNow(int bucketMinutes = 1)
    {
        var now = DateTime.Now;
        var minute = now.Minute - now.Minute % bucketMinutes;
        return new DateTime(now.Year, now.Month, now.Day, now.Hour, minute, 0, now.Kind);
    }
}
