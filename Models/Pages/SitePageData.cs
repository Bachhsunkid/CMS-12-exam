using System.ComponentModel.DataAnnotations;
using EPiServer.Web;

namespace TrainingTest.Models.Pages;

public class SitePageData : PageData
{
    [CultureSpecific]
    [Display(Name = "Meta title",
        Description = "Meta title is used for SEO and social media sharing. It should be unique for each page and between 5 and 60 characters long.",
        GroupName = Globals.GroupNames.MetaData, 
        Order = 100)]
    [StringLength(60, MinimumLength = 5)]
    public virtual string? MetaTitle { get; set; }

    [CultureSpecific]
    [Display(Name = "Meta keywords",
        Description = "Meta keywords are used for SEO. They should be relevant to the page content and separated by commas.",
        GroupName = Globals.GroupNames.MetaData, 
        Order = 200)]
    public virtual string? MetaKeywords { get; set; }

    [CultureSpecific]
    [Display(Name = "Meta description",
        Description = "Meta description is used for SEO and social media sharing. It should be a concise summary of the page content and between 50 and 160 characters long.",
        GroupName = Globals.GroupNames.MetaData, 
        Order = 300)]
    [UIHint(UIHint.Textarea)] // multi-row text editor
    public virtual string? MetaDescription { get; set; }
}