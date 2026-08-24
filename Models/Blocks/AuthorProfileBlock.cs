using System.ComponentModel.DataAnnotations;
using EPiServer.Web;

namespace TrainingTest.Models.Blocks;

[ContentType(
    DisplayName = "Author profile",
    Description = "Profile information for a blog author.",
    GUID = "23251f32-8b97-4a7b-8f54-6ad7c866f246",
    GroupName = Globals.GroupNames.Specialized,
    Order = 40)]
public class AuthorProfileBlock : SiteBlockData
{
    [CultureSpecific]
    [Required(AllowEmptyStrings = false)]
    [Display(Name = "Name", GroupName = Globals.GroupNames.Content, Order = 10)]
    public virtual string? FullName { get; set; }

    [CultureSpecific]
    [Required(AllowEmptyStrings = false)]
    [Display(Name = "Slug", GroupName = Globals.GroupNames.Content, Order = 20)]
    public virtual string? Slug { get; set; }

    [CultureSpecific]
    [UIHint(UIHint.Textarea)]
    [Display(Name = "Bio", GroupName = Globals.GroupNames.Content, Order = 30)]
    public virtual string? Bio { get; set; }

    [Display(Name = "Post count", GroupName = Globals.GroupNames.Publishing, Order = 10)]
    public virtual int PostCount { get; set; }
}
