using Eto.Drawing;
using Eto.Forms;

namespace RhinoFoundry.UI;

/// <summary>Neutral review surface. Consumers supply editable content and actions and retain all authorization policy.</summary>
public sealed class FoundryApprovalCard : Panel
{
    public FoundryApprovalCard(string title, Control reviewContent)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentNullException.ThrowIfNull(reviewContent);
        Padding = new Padding(1);
        BackgroundColor = FoundryTheme.CanvasBorder;
        Content = new StackLayout
        {
            BackgroundColor = FoundryTheme.PanelBackground,
            Padding = new Padding(FoundryTheme.Space3),
            Spacing = FoundryTheme.Space2,
            HorizontalContentAlignment = HorizontalAlignment.Stretch,
            Items =
            {
                new Label { Text = title, TextColor = FoundryTheme.PrimaryText, Font = FoundryTheme.BrandFont, TextAlignment = TextAlignment.Left },
                reviewContent,
            },
        };
    }
}
