using System.ComponentModel.DataAnnotations;

namespace TrainingTest.Models.Blocks;

[ContentType(
    DisplayName = "Header",
    Description = "Reusable site header.",
    GUID = "d2c72a24-1e21-4a59-9259-9de4d920bfd4",
    GroupName = Globals.GroupNames.Specialized,
    Order = 10)]
public class HeaderBlock : SiteBlockData
{
    [Required(AllowEmptyStrings = false)]
    [Display(Name = "Logo text", 
        GroupName = Globals.GroupNames.Content, 
        Order = 10)]
    public virtual required string LogoText { get; set; }

    [Display(Name = "Logo link", 
        GroupName = Globals.GroupNames.Content, 
        Order = 20)]
    public virtual Url? LogoLink { get; set; }

    [Display(Name = "Navigation links", 
        GroupName = Globals.GroupNames.Content, 
        Order = 40)]
    // public virtual LinkItemCollection? NavigationLinks { get; set; }
    public virtual ContentArea? NewNavigationLinks { get; set; } // use ContentArea for personalization
}
