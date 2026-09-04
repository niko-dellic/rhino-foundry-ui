using Xunit;
namespace RhinoFoundry.UI.Primitives.Tests;
public class SelectionContractTests
{
    [Fact] public void RangePreservesAnchorAndPrunesMissingKeys()
    {
        var model = new FoundrySelectionModel<int>();
        model.Replace([2], 2);
        model.SelectRange([1,2,3,4], 4, false);
        Assert.Equal(new[] {2,3,4}, model.VisibleSelection([1,2,3,4]));
        Assert.Equal(2, model.Anchor);
        model.Prune([4]);
        Assert.Equal(4, model.Anchor);
        model.Toggle(4);
        Assert.Empty(model.Selected);
        Assert.Null(model.Anchor);
    }
    [Fact] public void MissingRangeTargetDoesNotChangeSelection()
    {
        var model = new FoundrySelectionModel<int>();
        model.Replace([2], 2);
        model.SelectRange([1,2,3], 9, false);
        Assert.Equal(new[] {2}, model.Selected);
    }
}
