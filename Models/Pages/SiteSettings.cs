using System.ComponentModel.DataAnnotations;

namespace TrainingTest.Models.Pages;

public class SiteSettings : SitePageData
{
    [Display(Name = "Site Name",
        GroupName = Globals.GroupNames.SiteSettings, 
        Order = 400)]
    [Required(AllowEmptyStrings = false)]
    public virtual required string SiteName { get; set; }
    
    [Display(Name = "Default Header",
        GroupName = Globals.GroupNames.SiteSettings, 
        Order = 500)]
    public virtual ContentReference? DefaultHeader { get; set; }
    
    [Display(Name = "Default Footer",
        GroupName = Globals.GroupNames.SiteSettings, 
        Order = 600)]
    public virtual ContentReference? DefaultFooter { get; set; }
    
    [Display(Name = "Author Profile Folder",
        GroupName = Globals.GroupNames.SiteSettings, 
        Order = 700)]
    public virtual ContentReference? AuthorProfileFolder { get; set; }
}