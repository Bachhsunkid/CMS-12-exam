using System.Text.Json;
using TrainingTest.Business.Models;

namespace TrainingTest.Business.Blog;

public class BlogSeedDataProvider(IWebHostEnvironment webHostEnvironment) : IBlogSeedDataProvider
{
    // Loads and validates the deterministic JSON fixture used to create blog content.
    public BlogSeedData LoadSeedData()
    {
        var path = Path.Combine(webHostEnvironment.ContentRootPath, "SeedData", "blog-content.json");
        using var stream = File.OpenRead(path);
        var data = JsonSerializer.Deserialize<BlogSeedData>(stream, SerializerOptions)
            ?? throw new InvalidOperationException("Blog seed data is empty.");

        Validate(data);
        return data;
    }

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private static void Validate(BlogSeedData data)
    {
        if (data.Authors.Count == 0 || data.Posts.Count == 0)
        {
            throw new InvalidOperationException("Blog seed data must contain authors and posts.");
        }

        if (data.Authors.Select(author => author.Slug).Distinct(StringComparer.OrdinalIgnoreCase).Count() != data.Authors.Count ||
            data.Posts.Select(post => post.RouteSegment).Distinct(StringComparer.OrdinalIgnoreCase).Count() != data.Posts.Count)
        {
            throw new InvalidOperationException("Author slugs and blog post route segments must be unique.");
        }
    }
}
