using TrainingTest.Models.Blocks;

namespace TrainingTest.Business.Models;

public class LayoutModel
{
    public HeaderBlock? Header { get; set; } 
    public FooterBlock? Footer { get; set; }
    public string? SiteName { get; set; }
}
