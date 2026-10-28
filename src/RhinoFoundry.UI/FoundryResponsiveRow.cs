using Eto.Drawing;
using Eto.Forms;
using RhinoFoundry.UI;

namespace RhinoFoundry.UI;

/// <summary>Wraps controls only when their minimum usable widths no longer fit.</summary>
public sealed class FoundryResponsiveRow : PixelLayout
{
    public readonly record struct Item(Control Control, int MinimumWidth, bool Stretch = true);
    private readonly Item[] _items;

    public FoundryResponsiveRow(params Item[] items)
    {
        ArgumentNullException.ThrowIfNull(items);
        if (items.Any(item => item.Control is null || item.MinimumWidth < 1))
            throw new ArgumentException("Each item needs a control and a positive minimum width.", nameof(items));
        _items = items.ToArray();
        foreach (var item in items) Add(item.Control, 0, 0);
    }

    public void FitWidth(int width)
    {
        width = Math.Max(1, width);
        const int gap = FoundryTheme.Space2;
        Width = width;
        var y = 0;
        for (var first = 0; first < _items.Length;)
        {
            var end = first + 1;
            var minimum = _items[first].MinimumWidth;
            while (end < _items.Length && minimum + gap + _items[end].MinimumWidth <= width)
                minimum += gap + _items[end++].MinimumWidth;
            var flexible = _items.Skip(first).Take(end - first).Count(item => item.Stretch);
            var extra = Math.Max(0, width - minimum);
            var x = 0;
            var rowHeight = 0;
            var sizes = new List<(Item Item, int X, int Width, int Height)>();
            for (var i = first; i < end; i++)
            {
                var item = _items[i];
                var share = item.Stretch && flexible > 0 ? extra / flexible : 0;
                if (item.Stretch) { extra -= share; flexible--; }
                var w = Math.Min(width, item.MinimumWidth + share);
                item.Control.Width = w;
                item.Control.Height = -1;
                var h = Math.Max(1, (int)Math.Ceiling(item.Control.GetPreferredSize(new SizeF(w, -1)).Height));
                sizes.Add((item, x, w, h));
                rowHeight = Math.Max(rowHeight, h);
                x += w + gap;
            }
            foreach (var entry in sizes)
            {
                entry.Item.Control.Size = new Size(entry.Width, entry.Height);
                Move(entry.Item.Control, entry.X, y + rowHeight - entry.Height);
            }
            y += rowHeight + gap;
            first = end;
        }
        Height = Math.Max(1, y - gap);
    }
}
