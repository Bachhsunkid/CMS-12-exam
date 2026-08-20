using System.ComponentModel.DataAnnotations;

namespace TrainingTest.Models.Pages;

[ContentType(
    DisplayName = "Start page",
    Description = "The root page for Opti Blog.",
    GUID = "e19a2195-9d66-43c1-a956-0c6425cfe386",
    GroupName = Globals.GroupNames.Specialized,
    Order = 1)]
[AvailableContentTypes(Include = [typeof(BlogListPage)])]
public class StartPage : SitePageData
{
    [Display(Name = "Main content area", 
        GroupName = Globals.GroupNames.Content, 
        Order = 10)]
    public virtual ContentArea? MainContentArea { get; set; }
}