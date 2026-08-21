using System.ComponentModel.DataAnnotations;
using EPiServer.SpecializedProperties;

namespace TrainingTest.Models.Blocks;

[ContentType(
    DisplayName = "Footer",
    Description = "Reusable site footer.",
    GUID = "4690d2e6-bd82-49e5-9f6b-406909753c65",
    GroupName = Globals.GroupNames.Specialized,
    Order = 20)]
public class FooterBlock : SiteBlockData
{
    [Required(AllowEmptyStrings = false)]
    [Display(Name = "Copyright text", 
        GroupName = Globals.GroupNames.Content, 
        Order = 10)]
    public virtual required string CopyrightText { get; set; }

    [Display(Name = "Links", 
        GroupName = Globals.GroupNames.Content, 
        Order = 20)]
    public virtual LinkItemCollection? Links { get; set; }
}
