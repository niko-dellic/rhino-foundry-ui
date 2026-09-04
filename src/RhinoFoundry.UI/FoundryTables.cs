using Eto.Forms;
namespace RhinoFoundry.UI;
/// <summary>Native Eto table. Consumers retain ownership of columns, row data and commands.</summary>
public class FoundryGridView : GridView
{
    public FoundryGridView() { GridLines = GridLines.None; }
}
/// <summary>Native tree with shared platform presentation and selection restoration.</summary>
public class FoundryTreeGridView : TreeGridView
{
    public FoundryTreeGridView() { GridLines = GridLines.None; }
    public void ConfigureAlternatingRows() => FoundryNative.Services?.ConfigureAlternatingRows(this);
    public void RestoreSelectedRows(IReadOnlyList<int> rows) => FoundryNative.Services?.SelectRows(this, rows);
}
