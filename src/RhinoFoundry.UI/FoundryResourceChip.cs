using Eto.Forms;

namespace RhinoFoundry.UI;

/// <summary>Focusable resource reference. Consumers own navigation; Enter/Space activate the shared button.</summary>
public sealed class FoundryResourceChip : Panel
{
    public FoundryResourceChip(string kind, string name, Action activate)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(kind);
        ArgumentNullException.ThrowIfNull(activate);
        var button = new FoundryDialogButton($"{kind} · {name}", FoundryDialogButtonStyle.Secondary, 240)
        { ToolTip = $"Open {kind}: {name}" };
        button.Click += (_, _) => activate();
        Content = button;
    }
}
