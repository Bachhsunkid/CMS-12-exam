namespace TrainingTest.Business.Models;

public class BlogSeedData
{
    public List<AuthorSeedData> Authors { get; init; } = [];
    public List<BlogPostSeedData> Posts { get; init; } = [];
}

public class AuthorSeedData
{
    public required string Name { get; init; }
    public required string Slug { get; init; }
    public required string Bio { get; init; }
}

public class BlogPostSeedData
{
    public required string RouteSegment { get; init; }
    public required string Title { get; init; }
    public required string Summary { get; init; }
    public required string Body { get; init; }
    public int PublishOffsetDays { get; init; }
    public required string AuthorSlug { get; init; }
    public List<string> Tags { get; init; } = [];
}

public class BlogContentSeedResult
{
    public int AuthorsCreated { get; set; }
    public int PostsCreated { get; set; }
    public int PostsUnchanged { get; set; }
    public int Failed { get; set; }
    public bool Stopped { get; set; }
    public List<string> Messages { get; } = [];

    public string ToSummary() => string.Join("\n",
        new[]
        {
            $"Seed blog content {(Stopped ? "stopped" : "completed")}",
            $"Posts: {PostsCreated} created, {PostsUnchanged} unchanged",
            $"Authors: {AuthorsCreated} created; failed: {Failed}"
        }.Concat(Messages));
}
