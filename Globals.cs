using System.ComponentModel.DataAnnotations;

namespace TrainingTest;

public class Globals
{
    /// <summary>
    /// Group names for content types and properties
    /// </summary>
    [GroupDefinitions]
    public static class GroupNames
    {
        [Display(Name = "Default", Order = 10)]
        public const string Default = "Default";

        [Display(Name = SystemTabNames.Content, Order = 20)]
        public const string Content = SystemTabNames.Content;

        [Display(Name = SystemTabNames.Settings, Order = 30)]
        public const string Settings = SystemTabNames.Settings;

        [Display(Name = "Metadata", Order = 40)]
        public const string MetaData = "Metadata";

        [Display(Name = "News", Order = 50)]
        public const string News = "News";

        [Display(Name = "Products", Order = 60)]
        public const string Products = "Products";

        [Display(Name = "Contact", Order = 70)]
        public const string Contact = "Contact";

        [Display(Name = "SiteSettings", Order = 80)]
        public const string SiteSettings = "SiteSettings";

        [Display(Name = "Header and footer", Order = 85)]
        public const string HeaderAndFooter = "HeaderAndFooter";

        [Display(Name = "Specialized", Order = 90)]
        public const string Specialized = "Specialized";
        
        [Display(Name = "Publishing", Order = 100)]
        public const string Publishing = "Publishing";

        [Display(Name = "Relationships", Order = 110)]
        public const string Relationships = "Relationships";

        [Display(Name = "Editorial", Order = 120)]
        public const string Editorial = "Editorial";
    }
    
    public static class Layouts
    {
        public const string FullWidth = "full";
        public const string HalfWidth = "half";
        public const string Card = "card";
    }
}
