using Eto.Drawing;
using Eto.Forms;

namespace RhinoFoundry.UI;

public sealed class FoundryInsetFormField : PixelLayout
{
    private readonly Control _input;
    private readonly FieldChrome _chrome;

    public FoundryInsetFormField(Control input)
    {
        _input = input;
        _chrome = new FieldChrome();
        Height = 34;
        BackgroundColor = Colors.Transparent;
        PrepareInput(input);
        Add(_chrome, 0, 0);
        Add(_input, 8, 5);
        SizeChanged += (_, _) => LayoutChildren();
        input.GotFocus += (_, _) => { _chrome.Focused = true; _chrome.Invalidate(); };
        input.LostFocus += (_, _) => { _chrome.Focused = false; _chrome.Invalidate(); };
    }

    private void LayoutChildren()
    {
        _chrome.Size = ClientSize;
        _input.Size = new Size(Math.Max(0, ClientSize.Width - 16), 24);
        Move(_chrome, 0, 0);
        Move(_input, 8, 5);
    }

    private static void PrepareInput(Control input)
    {
        input.BackgroundColor = Colors.Transparent;
        if (input is TextBox textBox) textBox.ShowBorder = false;
        if (input is DropDown dropDown) dropDown.ShowBorder = false;
        if (input is NumericStepper numeric) numeric.TextColor = FoundrySurfaceTheme.Text;
    }

    private sealed class FieldChrome : Drawable
    {
        public bool Focused { get; set; }

        public FieldChrome() : base(false)
        {
            Paint += (_, eventArgs) =>
            {
                using var path = GraphicsPath.GetRoundRect(
                    new RectangleF(0.5f, 0.5f, Math.Max(0, Width - 1), Math.Max(0, Height - 1)), 6);
                eventArgs.Graphics.FillPath(FoundrySurfaceTheme.Surface, path);
                eventArgs.Graphics.DrawPath(new Pen(Focused ? FoundrySurfaceTheme.Focus : FoundrySurfaceTheme.Border, Focused ? 1.5f : 1), path);
            };
        }
    }
}
