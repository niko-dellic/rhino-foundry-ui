using Eto.Drawing;
using Eto.Forms;

namespace RhinoFoundry.UI;

/// <summary>
/// Compact toolbar field using the same quiet outlined shell as dialog inputs.
/// </summary>
public sealed class FoundryToolbarField : Panel
{
    public FoundryToolbarField(
        Control input,
        int width,
        Control? interactionControl = null)
    {
        ArgumentNullException.ThrowIfNull(input);
        var isSearchField = interactionControl is not null && !ReferenceEquals(input, interactionControl);
        Width = width;
        Height = 32;
        BackgroundColor = Colors.Transparent;
        Content = new FoundryFormField(
            input,
            interactionControl,
            minimumHeight: Height,
            horizontalInset: isSearchField ? 10 : null,
            cornerRadius: isSearchField ? 8 : 6);
    }
}
