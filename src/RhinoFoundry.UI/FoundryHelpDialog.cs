using Eto.Drawing;
using Eto.Forms;

namespace RhinoFoundry.UI;

/// <summary>Small Foundry-owned modal for contextual field and section help.</summary>
public sealed class FoundryHelpDialog : Dialog
{
    private readonly FoundryThemeBinding _themeBinding;
    public FoundryHelpDialog(string title, params string[] paragraphs)
    {
        ArgumentNullException.ThrowIfNull(paragraphs);
        Title = title;
        MinimumSize = new Size(420, 180);
        Resizable = false;
        Padding = new Padding(FoundryTheme.Space4);
        BackgroundColor = FoundryTheme.PanelBackground;

        var close = new FoundryDialogButton("Close", FoundryDialogButtonStyle.Secondary);
        close.Click += (_, _) => Close();
        FoundryDialogActions.Bind(this, close, close);

        var explanation = new StackLayout
        {
            Width = 388,
            Spacing = FoundryTheme.Space2,
            HorizontalContentAlignment = HorizontalAlignment.Stretch,
        };
        foreach (var paragraph in paragraphs.Where(text => !string.IsNullOrWhiteSpace(text)))
        {
            var label = FoundryTheme.MutedLabel(paragraph);
            label.Width = 388;
            label.Wrap = WrapMode.Word;
            explanation.Items.Add(label);
        }

        Content = new StackLayout
        {
            Spacing = FoundryTheme.Space3,
            HorizontalContentAlignment = HorizontalAlignment.Stretch,
            Items =
            {
                explanation,
                new StackLayoutItem(null, true),
                new StackLayout
                {
                    Orientation = Orientation.Horizontal,
                    Items = { new StackLayoutItem(null, true), close },
                },
            },
        };
        _themeBinding = new FoundryThemeBinding(this, () =>
        {
            BackgroundColor = FoundryTheme.PanelBackground;
            foreach (var label in explanation.Children.OfType<Label>()) label.TextColor = FoundryTheme.MutedText;
        });
    }
    protected override void Dispose(bool disposing)
    {
        if (IsDisposed) return;
        if (disposing) _themeBinding.Dispose();
        base.Dispose(disposing);
    }

}
