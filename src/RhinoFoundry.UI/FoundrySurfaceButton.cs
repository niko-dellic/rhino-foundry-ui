using Eto.Drawing;
using Eto.Forms;

namespace RhinoFoundry.UI;

public sealed class FoundrySurfaceButton : Drawable
{
    private readonly Font _font = SystemFonts.Bold(9);
    private bool _hovered;
    private bool _pressed;
    private bool _focused;

    public FoundrySurfaceButton(string text, int width = 110) : base(true)
    {
        Text = text;
        MinimumSize = new Size(width, 34);
        Height = 34;
        CanFocus = true;
        ToolTip = text;
        Paint += OnPaint;
        MouseEnter += (_, _) => { _hovered = true; Invalidate(); };
        MouseLeave += (_, _) => { _hovered = false; _pressed = false; Invalidate(); };
        MouseDown += (_, eventArgs) =>
        {
            if (!Enabled || !eventArgs.Buttons.HasFlag(MouseButtons.Primary)) return;
            _pressed = true;
            Focus();
            eventArgs.Handled = true;
            Invalidate();
        };
        MouseUp += (_, eventArgs) =>
        {
            if (!Enabled || !_pressed) return;
            _pressed = false;
            Click?.Invoke(this, EventArgs.Empty);
            eventArgs.Handled = true;
            Invalidate();
        };
        KeyDown += (_, eventArgs) =>
        {
            if (!Enabled || eventArgs.Key is not (Keys.Enter or Keys.Space)) return;
            Click?.Invoke(this, EventArgs.Empty);
            eventArgs.Handled = true;
        };
        GotFocus += (_, _) => { _focused = true; Invalidate(); };
        LostFocus += (_, _) => { _focused = false; _pressed = false; Invalidate(); };
        EnabledChanged += (_, _) => Invalidate();
    }

    public string Text { get; set; }
    public bool Active { get; set; }
    public event EventHandler? Click;

    private void OnPaint(object? sender, PaintEventArgs eventArgs)
    {
        var bounds = new RectangleF(0.5f, 0.5f, Math.Max(0, Width - 1), Math.Max(0, Height - 1));
        using var path = GraphicsPath.GetRoundRect(bounds, 6);
        var fill = Active ? FoundrySurfaceTheme.Active : FoundrySurfaceTheme.Surface;
        if (_hovered && Enabled) fill = FoundrySurfaceTheme.HoverSurface;
        if (_pressed && Enabled) fill = FoundrySurfaceTheme.Active;
        eventArgs.Graphics.FillPath(fill, path);
        eventArgs.Graphics.DrawPath(new Pen(FoundrySurfaceTheme.WithAlpha(FoundrySurfaceTheme.Border, Enabled ? 210 : 80), 1), path);
        var color = Enabled ? FoundrySurfaceTheme.Text : FoundrySurfaceTheme.MutedText;
        var size = eventArgs.Graphics.MeasureString(_font, Text);
        eventArgs.Graphics.DrawText(_font, color, (Width - size.Width) / 2, (Height - size.Height) / 2, Text);
        if (_focused && Enabled)
        {
            using var focus = GraphicsPath.GetRoundRect(new RectangleF(2.5f, 2.5f, Width - 5, Height - 5), 4);
            eventArgs.Graphics.DrawPath(new Pen(FoundrySurfaceTheme.WithAlpha(FoundrySurfaceTheme.Focus, 180), 1), focus);
        }
    }
}
