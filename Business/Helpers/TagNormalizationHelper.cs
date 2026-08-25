namespace TrainingTest.Business.Helpers;

public static class TagNormalizationHelper
{
    public static IReadOnlyList<string> Normalize(IEnumerable<string>? tags)
    {
        if (tags is null)
        {
            return [];
        }

        return tags
            .Select(tag => tag.Trim().ToLowerInvariant())
            .Where(tag => !string.IsNullOrWhiteSpace(tag))
            .Distinct(StringComparer.Ordinal)
            .ToList();
    }
}
