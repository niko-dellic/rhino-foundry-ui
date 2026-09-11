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

    public static bool FormatCell(GridCellFormatEventArgs args, bool selected) => FormatCell(args, selected, null);

    public static bool FormatCell(GridCellFormatEventArgs args, bool selected, Color? selectionColor)
    {
        args.Font = FoundryTheme.HierarchyTableFont;
        args.ForegroundColor = selected ? selectionColor is { } accent ? SelectionForeground(accent) : SystemColors.SelectionText : FoundryTheme.PrimaryText;
        // The native Mac row owns its background throughout mouse tracking.
        // CellFormatting/selection notifications are not a live painting contract.
        // A clear cell fill uses native copy compositing and erases our custom
        // row selection. Leave cell backgrounds unset for the opt-in row painter.
        if (!OperatingSystem.IsMacOS())
            args.BackgroundColor = selected ? selectionColor ?? SystemColors.Selection
                : args.Row % 2 == 0 ? FoundryTheme.ContentBackground
                : FoundryTheme.HierarchyAlternateRowBackground;
        else if (selectionColor is null)
            args.BackgroundColor = Colors.Transparent;
        return selected;
    }

    /// <summary>Configure once during composition. The getter runs on the UI thread;
    /// reload the table when its value changes. Other tables retain native colors.</summary>
    public static void ConfigureSelectionColor(TreeGridView tree, Func<Color> color)
    {
        tree.Load += (_, _) => FoundryNative.Services?.ConfigureSelectionColor(tree, color);
    }

    /// <summary>Contrasting foreground for an opaque custom selection.</summary>
    public static Color SelectionForeground(Color color)
    {
        static double Linear(int channel)
        {
            var value = channel / 255.0;
            return value <= 0.04045 ? value / 12.92 : Math.Pow((value + 0.055) / 1.055, 2.4);
        }
        var luminance = 0.2126 * Linear(color.Rb) + 0.7152 * Linear(color.Gb) + 0.0722 * Linear(color.Bb);
        return luminance > 0.179 ? Colors.Black : Colors.White;
    }

    public static void SetCellBackground(GridCellFormatEventArgs args, Color color)
    {
        // Semantic cells keep their foreground/font cues on Mac without hiding
        // the native selection. Other platforms retain their background accents.
        if (!OperatingSystem.IsMacOS()) args.BackgroundColor = color;
    }

}
