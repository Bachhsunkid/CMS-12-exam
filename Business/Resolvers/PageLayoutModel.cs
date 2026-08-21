using TrainingTest.Models.Blocks;

namespace TrainingTest.Business.Resolvers;

public class PageLayoutModel
{
    public HeaderBlock? Header { get; set; } 
    public FooterBlock? Footer { get; set; }
}
