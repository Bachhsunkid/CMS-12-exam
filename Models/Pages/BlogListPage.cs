using System.ComponentModel.DataAnnotations;

namespace TrainingTest.Models.Pages;

[ContentType(DisplayName = "Blog List Page", 
    GUID = "f7d4e443-a708-4af1-8927-fe1c13ad6fa1",
    Description = "Page that lists blog posts.",
    GroupName = Globals.GroupNames.Specialized,
    Order = 10)]
[AvailableContentTypes(Include = [typeof(BlogPostPage)])]
public class BlogListPage : SitePageData
{
    [CultureSpecific]
    [Required(AllowEmptyStrings = false)]
    [Display(Name = "Heading", 
        GroupName = Globals.GroupNames.Content, 
        Order = 10)]
    public virtual string? Heading { get; set; }

    [CultureSpecific]
    [Display(Name = "Intro", 
        GroupName = Globals.GroupNames.Content, 
        Order = 20)]
    public virtual XhtmlString? Intro { get; set; }
    
    [AllowedTypes(typeof(Blocks.BlogTeaserBlock))]
    [Display(Name = "Main content area", 
        GroupName = Globals.GroupNames.Content, 
        Order = 30)]
    public virtual ContentArea? MainContentArea { get; set; }

    [Range(1, 50)]
    [Display(
        Name = "Page size",
        Description = "Number of posts shown on each page. Use a value between 1 and 50.",
        GroupName = Globals.GroupNames.Settings,
        Order = 10)]
    public virtual int? PageSize { get; set; }
    
    public override void SetDefaultValues(ContentType contentType)
    {
        base.SetDefaultValues(contentType);
        PageSize = Constants.DefaultPageSize;
    }
}
