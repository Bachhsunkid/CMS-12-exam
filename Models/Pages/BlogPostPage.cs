using System.ComponentModel.DataAnnotations;
using EPiServer.SpecializedProperties;
using EPiServer.Web;

namespace TrainingTest.Models.Pages;

[ContentType(DisplayName = "Blog Post Page",
    GUID = "b8edf6f4-1af3-40ef-af38-a2511a24d54c",
    Description = "Page that represents a single blog post.",
    GroupName = Globals.GroupNames.Specialized,
    Order = 20)]
[AvailableContentTypes(IncludeOn = [typeof(BlogListPage)])]
public class BlogPostPage : SitePageData
{
    [CultureSpecific]
    [Required(AllowEmptyStrings = false)]
    [Display(Name = "Title", 
        GroupName = Globals.GroupNames.Content, 
        Order = 10)]
    public virtual string? Title { get; set; }

    [CultureSpecific]
    [Required(AllowEmptyStrings = false)]
    [StringLength(300)]
    [UIHint(UIHint.Textarea)]
    [Display(
        Name = "Summary",
        Description = "Plain text teaser with a maximum of 300 characters.",
        GroupName = Globals.GroupNames.Content,
        Order = 20)]
    public virtual string? Summary { get; set; }

    [CultureSpecific]
    [Display(Name = "Main body", 
        GroupName = Globals.GroupNames.Content, 
        Order = 30)]
    public virtual XhtmlString? MainBody { get; set; }
    
    [UIHint(UIHint.Image)]
    [AllowedTypes(typeof(Media.ImageFile))]
    [Display(Name = "Hero image", 
        GroupName = Globals.GroupNames.Content, 
        Order = 40)]
    public virtual ContentReference? HeroImage { get; set; }

    [Required]
    [Display(Name = "Publish date", 
        GroupName = Globals.GroupNames.Publishing, 
        Order = 10)]
    public virtual DateTime? PublishDate { get; set; }

    [CultureSpecific]
    [Required(AllowEmptyStrings = false)]
    [Display(Name = "Author", 
        GroupName = Globals.GroupNames.Publishing, 
        Order = 20)]
    public virtual string? Author { get; set; }

    [CultureSpecific]
    [BackingType(typeof(PropertyStringList))]
    [Display(Name = "Tags", 
        GroupName = Globals.GroupNames.Publishing, 
        Order = 30)]
    public virtual IList<string>? Tags { get; set; }

    [AllowedTypes(typeof(Blocks.BlogTeaserBlock))]
    [Display(Name = "Related posts", 
        GroupName = Globals.GroupNames.Relationships, 
        Order = 10)]
    public virtual ContentArea? RelatedPosts { get; set; }

    [CultureSpecific]
    [UIHint(UIHint.Textarea)]
    [Display(
        Name = "Internal notes",
        Description = "Editorial notes. This field is never rendered to visitors.",
        GroupName = Globals.GroupNames.Editorial,
        Order = 10)]
    public virtual string? InternalNotes { get; set; }
    
    public override void SetDefaultValues(ContentType contentType)
    {
        base.SetDefaultValues(contentType);
        PublishDate = DateTime.UtcNow;
    }
}
