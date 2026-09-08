using Eto.Drawing;
using Eto.Forms;

namespace RhinoFoundry.UI;

/// <summary>A product-neutral chronological activity surface. Hosts own scrolling and callbacks.</summary>
public sealed class FoundryActivityTimeline : StackLayout
{
    public FoundryActivityTimeline()
    {
        Spacing = FoundryTheme.Space2;
        HorizontalContentAlignment = HorizontalAlignment.Stretch;
    }

    /// <summary>Adds a factual activity row. Call on the UI thread. Images remain caller-owned.</summary>
    public void AddActivity(string title, string state, string? detail = null, Image? image = null)
    {
        var row = new FoundryActivityCard(title, state, detail, image);
        Items.Add(row);
    }
}

/// <summary>Quiet activity card with an optional image preview; contains no workflow behavior.</summary>
public sealed class FoundryActivityCard : Panel
{
    public FoundryActivityCard(string title, string state, string? detail = null, Image? image = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        BackgroundColor = FoundryTheme.CanvasBorder;
        Padding = new Padding(1);
        var body = new StackLayout
        {
            Padding = new Padding(FoundryTheme.Space3),
            Spacing = FoundryTheme.Space2,
            BackgroundColor = FoundryTheme.PanelBackground,
            HorizontalContentAlignment = HorizontalAlignment.Stretch,
        };
        body.Items.Add(new Label { Text = title, TextColor = FoundryTheme.PrimaryText, Wrap = WrapMode.Word });
        body.Items.Add(FoundryTheme.MutedLabel(state));
        if (!string.IsNullOrWhiteSpace(detail))
            body.Items.Add(new Label { Text = detail, TextColor = FoundryTheme.PrimaryText, Wrap = WrapMode.Word });
        if (image is not null)
        {
            body.Items.Add(new ImageView { Image = image, Height = 180 });
            var expand = new FoundryDialogButton("Inspect image", FoundryDialogButtonStyle.Secondary);
            expand.Click += (_, _) =>
            {
                var close = new FoundryDialogButton("Close", FoundryDialogButtonStyle.Secondary);
                var dialog = new Dialog { Title = title, Resizable = true, Size = new Size(800, 650) };
                close.Click += (_, _) => dialog.Close();
                dialog.Content = new StackLayout
                {
                    HorizontalContentAlignment = HorizontalAlignment.Stretch,
                    Items = { new StackLayoutItem(new ImageView { Image = image }, true), close },
                };
                FoundryDialogActions.Bind(dialog, null, close);
                dialog.ShowModal(ParentWindow);
                dialog.Dispose();
            };
            body.Items.Add(expand);
        }
        Content = body;
    }
}
