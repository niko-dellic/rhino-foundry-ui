using Eto.Drawing;
using Eto.Forms;

namespace RhinoFoundry.UI;

/// <summary>Content-height table with per-column wrapping and no nested scroll view.
/// Values are copied. Optional row actions run asynchronously on the UI thread.</summary>
public sealed class FoundryReadOnlyTable : PixelLayout
{
    public FoundryReadOnlyTable(IReadOnlyList<string> headers, IReadOnlyList<string[]> rows,
        IReadOnlyList<bool>? wrapColumns = null, Action<int>? rowAction = null)
    {
        if (headers.Count == 0) throw new ArgumentException("At least one column is required.", nameof(headers));
        if (wrapColumns is not null && wrapColumns.Count != headers.Count)
            throw new ArgumentException("Provide one wrapping flag per column.", nameof(wrapColumns));
        var values = new[] { headers.ToArray() }.Concat(rows.Select(r => r.ToArray())).ToArray();
        var labels = new List<(Label Label, int Row, int Column)>();
        var backgrounds = new List<Panel>();
        var actions = new List<FoundryDialogButton>();
        for (var r = 0; r < values.Length; r++)
        {
            var background = new Panel { BackgroundColor = r == 0 || r % 2 == 1
                ? FoundryTheme.ContentBackground : FoundryTheme.HierarchyAlternateRowBackground };
            backgrounds.Add(background); Add(background, 0, 0);
            for (var c = 0; c < headers.Count; c++)
            {
                var value = c < values[r].Length ? values[r][c] : "";
                var label = new Label { Text = value, ToolTip = value,
                    Wrap = r == 0 || wrapColumns?[c] == true ? WrapMode.Word : WrapMode.None,
                    TextColor = FoundryTheme.PrimaryText, TextAlignment = TextAlignment.Left,
                    Font = r == 0 ? SystemFonts.Bold() : FoundryTheme.HierarchyTableFont };
                label.LoadComplete += (_, _) => { label.TextAlignment = TextAlignment.Right; label.TextAlignment = TextAlignment.Left; };
                labels.Add((label, r, c)); Add(label, 0, 0);
            }
            if (r > 0 && rowAction is not null)
            {
                var index = r - 1;
                var open = new FoundryDialogButton("Open", FoundryDialogButtonStyle.Secondary, 64);
                open.Click += (_, _) => Application.Instance.AsyncInvoke(() => { if (!IsDisposed) rowAction(index); });
                actions.Add(open); Add(open, 0, 0);
            }
        }
        var queued = false;
        var lastWidth = -1;
        void Fit()
        {
            if (queued || IsDisposed) return;
            queued = true;
            Application.Instance.AsyncInvoke(() =>
            {
                queued = false;
                if (IsDisposed || ClientSize.Width <= 0 || lastWidth == ClientSize.Width) return;
                lastWidth = ClientSize.Width;
                var available = Math.Max(headers.Count, lastWidth - (rowAction is null ? 0 : 72));
                var columnWidth = Math.Max(1, available / headers.Count);
                var y = 0;
                for (var r = 0; r < values.Length; r++)
                {
                    var height = rowAction is null ? FoundryTheme.TableRowHeight : 40;
                    foreach (var cell in labels.Where(l => l.Row == r))
                    {
                        cell.Label.Height = -1;
                        cell.Label.Width = Math.Max(1, columnWidth - FoundryTheme.Space3 * 2);
                        height = Math.Max(height, (int)Math.Ceiling(cell.Label.GetPreferredSize(new Size(cell.Label.Width, 100000)).Height) + FoundryTheme.Space2 * 2);
                    }
                    backgrounds[r].Size = new Size(lastWidth, height); Move(backgrounds[r], 0, y);
                    foreach (var cell in labels.Where(l => l.Row == r))
                    {
                        cell.Label.Height = height - FoundryTheme.Space2 * 2;
                        Move(cell.Label, cell.Column * columnWidth + FoundryTheme.Space3, y + FoundryTheme.Space2);
                    }
                    if (r > 0 && rowAction is not null) Move(actions[r - 1], lastWidth - 68, y + 4);
                    y += height;
                }
                Height = y;
            });
        }
        SizeChanged += (_, _) => Fit(); LoadComplete += (_, _) => Fit();
    }
}
