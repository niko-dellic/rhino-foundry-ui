using Eto.Drawing;
using Eto.Forms;

namespace RhinoFoundry.UI;

/// <summary>Shared presentation for flat and hierarchical native tables.
/// Call FormatCell before adding operation-specific cell decoration; selection wins.</summary>
public static class FoundryTable
{
    public static void Configure(Grid grid)
    {
        grid.RowHeight = FoundryTheme.TableRowHeight;
        grid.ShowHeader = true;
        grid.GridLines = GridLines.None;
        grid.Load += (_, _) => FoundryNative.Services?.ConfigureAlternatingRows(grid);
    }

    public static bool FormatCell(GridCellFormatEventArgs args, bool selected)
    {
        args.Font = FoundryTheme.HierarchyTableFont;
        args.ForegroundColor = selected ? SystemColors.SelectionText : FoundryTheme.PrimaryText;
        // The native Mac row owns its background throughout mouse tracking.
        // CellFormatting/selection notifications are not a live painting contract.
        args.BackgroundColor = OperatingSystem.IsMacOS() ? Colors.Transparent
            : selected ? SystemColors.Selection
            : args.Row % 2 == 0 ? FoundryTheme.ContentBackground
            : FoundryTheme.HierarchyAlternateRowBackground;
        return selected;
    }

    public static void SetCellBackground(GridCellFormatEventArgs args, Color color)
    {
        // Semantic cells keep their foreground/font cues on Mac without hiding
        // the native selection. Other platforms retain their background accents.
        if (!OperatingSystem.IsMacOS()) args.BackgroundColor = color;
    }

}
