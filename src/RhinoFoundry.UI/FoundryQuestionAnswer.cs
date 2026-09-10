using Eto.Drawing;
using Eto.Forms;

namespace RhinoFoundry.UI;

/// <summary>One full-width answer target with a title, description and recommendation badge.</summary>
internal sealed class FoundryQuestionAnswer : Drawable
{
    private bool _hovered, _pressed;
    private readonly int _number;
    private readonly string _title, _description;
    private readonly bool _recommended;
    public event EventHandler? Click;

    internal FoundryQuestionAnswer(int number, string title, string description) : base(true)
    {
        _number = number;
        _recommended = title.EndsWith("(Recommended)", StringComparison.OrdinalIgnoreCase);
        _title = _recommended ? title[..^13].TrimEnd() : title;
        _description = description;
        Height = string.IsNullOrWhiteSpace(description) ? 48 : 88;
        CanFocus = true;
        ToolTip = title + (string.IsNullOrWhiteSpace(description) ? "" : "\n" + description);
        MouseEnter += (_, _) => { _hovered = Enabled; Invalidate(); };
        MouseLeave += (_, _) => { _hovered = _pressed = false; Invalidate(); };
        MouseDown += (_, e) => { if (!Enabled || !e.Buttons.HasFlag(MouseButtons.Primary)) return; _pressed = true; Focus(); e.Handled = true; Invalidate(); };
        MouseUp += (_, e) => { if (!Enabled || !_pressed) return; _pressed = false; e.Handled = true; Invalidate(); Click?.Invoke(this, EventArgs.Empty); };
        KeyDown += (_, e) => { if (!Enabled || e.Key is not (Keys.Enter or Keys.Space)) return; e.Handled = true; Click?.Invoke(this, EventArgs.Empty); };
        GotFocus += (_, _) => Invalidate();
        LostFocus += (_, _) => { _pressed = false; Invalidate(); };
        EnabledChanged += (_, _) => { _hovered = _pressed = false; Invalidate(); };
        Paint += (_, e) => Draw(e.Graphics);
    }

    private void Draw(Graphics g)
    {
        using var shape = GraphicsPath.GetRoundRect(new RectangleF(.5f, .5f, Width - 1, Height - 1), 6);
        if (_hovered || _pressed) g.FillPath(FoundryTheme.WithAlpha(FoundryTheme.CanvasSubtleSurface, _pressed ? 230 : 135), shape);
        if (HasFocus && Enabled) { using var pen = new Pen(FoundryTheme.PrimaryText, 1); g.DrawPath(pen, shape); }
        var primary = Enabled ? FoundryTheme.PrimaryText : FoundryTheme.MutedText;
        // Eto caches system fonts; disposing them breaks subsequent native paints.
        var font = SystemFonts.Default();
        var bold = SystemFonts.Bold();
        using var border = new Pen(FoundryTheme.CanvasBorder, 1);
        g.DrawEllipse(border, 10, 10, 28, 28);
        g.DrawText(font, FoundryTheme.SecondaryText, 19, 15, _number.ToString());
        var available = Math.Max(20, Width - 90 - (_recommended ? 112 : 0));
        var title = Fit(g, bold, _title, available);
        g.DrawText(bold, primary, 48, 12, title);
        if (_recommended)
        {
            var x = 56 + g.MeasureString(bold, title).Width;
            using var badge = GraphicsPath.GetRoundRect(new RectangleF(x, 10, 104, 22), 6);
            g.FillPath(FoundryTheme.CanvasSubtleSurface, badge);
            g.DrawText(font, FoundryTheme.SecondaryText, x + 7, 13, "Recommended");
        }
        var words = _description.Split(' ');
        var line = "";
        var y = 36;
        foreach (var word in words)
        {
            var candidate = line.Length == 0 ? word : line + " " + word;
            if (g.MeasureString(font, candidate).Width > Math.Max(20, Width - 92) && line.Length > 0 && y < 54)
            { g.DrawText(font, FoundryTheme.SecondaryText, 48, y, line); y += 18; line = word; }
            else line = candidate;
        }
        g.DrawText(font, FoundryTheme.SecondaryText, 48, y, Fit(g, font, line, Math.Max(20, Width - 92)));
        if (_hovered || HasFocus) g.DrawText(font, primary, Width - 28, 17, "→");
    }

    private static string Fit(Graphics g, Font font, string text, float width)
    {
        if (g.MeasureString(font, text).Width <= width) return text;
        while (text.Length > 0 && g.MeasureString(font, text + "…").Width > width) text = text[..^1];
        return text + "…";
    }
}
