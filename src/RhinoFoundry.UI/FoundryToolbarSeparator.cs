using Eto.Drawing;
using Eto.Forms;

namespace RhinoFoundry.UI;

/// <summary>A theme-aware 1 × 20 rule centered in a 32px toolbar slot.</summary>
public sealed class FoundryToolbarSeparator : Panel
{
    private readonly Panel _rule = new() { Width = 1, Height = 20, BackgroundColor = FoundryTheme.CanvasBorder };
    private readonly FoundryThemeBinding _themeBinding;

    public FoundryToolbarSeparator()
    {
        Width = 1;
        Height = 32;
        Padding = new Padding(0, 6);
        BackgroundColor = Colors.Transparent;
        Content = _rule;
        _themeBinding = new FoundryThemeBinding(this, () => _rule.BackgroundColor = FoundryTheme.CanvasBorder);
    }

    protected override void Dispose(bool disposing)
    {
        if (IsDisposed) return;
        if (disposing) _themeBinding.Dispose();
        base.Dispose(disposing);
    }
}
