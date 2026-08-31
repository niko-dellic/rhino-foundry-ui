using Eto.Forms;

namespace RhinoFoundry.UI;

/// <summary>
/// Standard separator for 32-pixel Foundry toolbar rows.
/// </summary>
public sealed class FoundryToolbarSeparator : Panel
{
    public FoundryToolbarSeparator()
    {
        Width = 1;
        Height = 20;
        BackgroundColor = FoundryTheme.CanvasBorder;
    }
}
