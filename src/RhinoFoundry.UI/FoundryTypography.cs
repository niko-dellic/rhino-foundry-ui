using Eto.Forms;

namespace RhinoFoundry.UI;

/// <summary>Opt-in form typography. Preserves emphasis while giving native
/// labels, editors and disclosure headings the same body size.</summary>
public static class FoundryTypography
{
    /// <summary>Apply during composition, before showing a Windows form.
    /// Custom-drawn controls retain their own typography roles.</summary>
    public static void ApplyForm(Control root)
    {
        ArgumentNullException.ThrowIfNull(root);
        if (!OperatingSystem.IsWindows()) return;
        if (root is CommonControl control && root is not Drawable && root is not LinkButton)
            control.Font = control.Font.Bold ? FoundryTheme.FormEmphasisFont : FoundryTheme.FormFont;
        if (root is Expander)
        {
            FoundryWindowsPresentation.FormText(root);
            root.LoadComplete += (_, _) => Application.Instance.AsyncInvoke(() =>
            {
                if (!root.IsDisposed) FoundryWindowsPresentation.FormText(root);
            });
        }
        if (root is Container container)
            foreach (var child in container.Controls) ApplyForm(child);
    }
}
