using System.ComponentModel.DataAnnotations;

namespace TrainingTest.Models.Blocks;

[ContentType(
    DisplayName = "Blog teaser",
    Description = "Reusable teaser that links to a blog post.",
    GUID = "8b84f414-7b59-4dca-9563-e811abea85bd",
    GroupName = Globals.GroupNames.Specialized,
    Order = 30)]
public class BlogTeaserBlock : SiteBlockData
{
    [Required]
    [AllowedTypes(typeof(Pages.BlogPostPage))]
    [Display(Name = "Post", 
        GroupName = Globals.GroupNames.Content, 
        Order = 10)]
    public virtual ContentReference? Post { get; set; }

    [CultureSpecific]
    [StringLength(80)]
    [Display(Name = "Kicker", 
        GroupName = Globals.GroupNames.Content, 
        Order = 20)]
    public virtual string? Kicker { get; set; }
}
