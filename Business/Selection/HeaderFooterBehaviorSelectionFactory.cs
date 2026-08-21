using EPiServer.Shell.ObjectEditing;
using TrainingTest.Models.Pages;

namespace TrainingTest.Business.Selection;

public class HeaderFooterBehaviorSelectionFactory : ISelectionFactory
{
    // SelectOne supports int values; resolvers cast the selected value back to HeaderFooterBehavior.
    public IEnumerable<ISelectItem> GetSelections(ExtendedMetadata metadata) =>
    [
        new SelectItem { Text = nameof(HeaderFooterBehavior.Inherit), Value = (int)HeaderFooterBehavior.Inherit },
        new SelectItem { Text = nameof(HeaderFooterBehavior.Override), Value = (int)HeaderFooterBehavior.Override },
        new SelectItem { Text = nameof(HeaderFooterBehavior.Hidden), Value = (int)HeaderFooterBehavior.Hidden }
    ];
}
