using Eto.Drawing;
using Eto.Forms;

namespace RhinoFoundry.UI;

/// <summary>Read-only native table using the same presentation as Foundry hierarchy tables.
/// Rows are borrowed immutable values. Double-click a row to inspect its full text.</summary>
public sealed class FoundryReadOnlyTable : Panel
{
    public FoundryReadOnlyTable(IReadOnlyList<string> headers, IReadOnlyList<string[]> rows)
    {
        var grid = new GridView { AllowMultipleSelection = false };
        FoundryTable.Configure(grid);
        for (var i = 0; i < headers.Count; i++)
            grid.Columns.Add(new GridColumn { HeaderText = headers[i], DataCell = new TextBoxCell(i), Editable = false,
                Width = i == headers.Count - 1 ? 360 : 220, Resizable = true, AutoSize = false });
        grid.DataStore = rows;
        var queued = false;
        var lastWidth = -1;
        void FitColumns()
        {
            if (queued || IsDisposed) return;
            queued = true;
            Application.Instance.AsyncInvoke(() =>
            {
                queued = false;
                // Size to the table, not its enclosing panel. Native scroll views
                // also need room for their gutter, border and column spacing.
                var width = grid.Bounds.Width - FoundryTheme.Space4 - headers.Count * FoundryTheme.Space1;
                if (IsDisposed || width <= 0 || width == lastWidth || headers.Count == 0) return;
                lastWidth = width;
                var leading = Math.Max(1, Math.Min(320, width / (headers.Count + 1)));
                for (var i = 0; i < headers.Count; i++)
                    grid.Columns[i].Width = i == headers.Count - 1 ? Math.Max(1, width - leading * (headers.Count - 1)) : leading;
            });
        }
        SizeChanged += (_, _) => FitColumns();
        grid.SizeChanged += (_, _) => FitColumns();
        LoadComplete += (_, _) => FitColumns();
        grid.Height = Math.Min(420, 48 + rows.Count * FoundryTheme.TableRowHeight);
        grid.CellFormatting += (_, args) => FoundryTable.FormatCell(args, ReferenceEquals(args.Item, grid.SelectedItem));
        string Describe(string[] row) => string.Join("\n\n", headers.Select((header, index) => header + ": " + (index < row.Length ? row[index] : "")));
        grid.SelectionChanged += (_, _) => grid.ToolTip = grid.SelectedItem is string[] row ? Describe(row) : "Double-click a row to inspect its full text.";
        grid.CellDoubleClick += (_, args) =>
        {
            if (args.Item is not string[] row) return;
            using var dialog = new Dialog { Title = "Table row", Size = new Size(620, 420), Resizable = true };
            var close = new FoundryDialogButton("Close", FoundryDialogButtonStyle.Secondary);
            close.Click += (_, _) => dialog.Close();
            dialog.Content = new StackLayout { Padding = FoundryTheme.Space3, HorizontalContentAlignment = HorizontalAlignment.Stretch,
                Items = { new StackLayoutItem(new FoundryScrollable(new FoundryChatMessage(Describe(row), false)), true), close } };
            FoundryDialogActions.Bind(dialog, null, close);
            dialog.ShowModal(ParentWindow);
        };
        Content = grid;
        Height = grid.Height;
    }
}
