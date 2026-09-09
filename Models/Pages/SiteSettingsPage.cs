using System.ComponentModel.DataAnnotations;
using EPiServer.Web;
using TrainingTest.Models.Blocks;

namespace TrainingTest.Models.Pages;

// This page is provisioned by SiteSettingsInitializer, not created from the CMS New Page menu.
[ContentType(DisplayName = "Site settings",
    Description = "Shared settings and default page layout for one site.",
    GUID = "f5c673c4-4e3f-4b37-bf09-73d2a269d839",
    GroupName = Globals.GroupNames.Specialized,
    AvailableInEditMode = false,
    Order = 2)]
[AvailableContentTypes(IncludeOn = [typeof(StartPage)])]
public class SiteSettingsPage : PageData
{
    [Required(AllowEmptyStrings = false)]
    [Display(Name = "Site name",
        GroupName = Globals.GroupNames.SiteSettings,
        Order = 10)]
    public virtual required string SiteName { get; set; }

    [AllowedTypes(typeof(HeaderBlock))]
    [Display(Name = "Default header",
        GroupName = Globals.GroupNames.SiteSettings,
        Order = 20)]
    public virtual ContentReference? DefaultHeader { get; set; }

    [AllowedTypes(typeof(FooterBlock))]
    [Display(Name = "Default footer",
        GroupName = Globals.GroupNames.SiteSettings,
        Order = 30)]
    public virtual ContentReference? DefaultFooter { get; set; }

    [UIHint(UIHint.AssetsFolder)]
    [Display(Name = "Author profile folder",
        GroupName = Globals.GroupNames.SiteSettings,
        Order = 40)]
    public virtual ContentReference? AuthorProfileFolder { get; set; }

    public override void SetDefaultValues(ContentType contentType)
    {
        base.SetDefaultValues(contentType);
        VisibleInMenu = false;
    }
}
