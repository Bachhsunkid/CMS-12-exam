using TrainingTest.Models.Pages;

namespace TrainingTest.Models.ViewModels;

public record BlogPostListItemViewModel (BlogPostPage Post, string? AuthorName, string? AuthorUrl);