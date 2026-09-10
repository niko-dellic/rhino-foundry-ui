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
        BackgroundColor = FoundryTheme.PanelBackground;
        Padding = new Padding(0);
        var body = new StackLayout
        {
            Padding = new Padding(FoundryTheme.Space2, FoundryTheme.Space1),
            Spacing = FoundryTheme.Space2,
            BackgroundColor = FoundryTheme.PanelBackground,
            HorizontalContentAlignment = HorizontalAlignment.Stretch,
        };
        var labels = new List<Label>();
        Label Text(string value, Color color)
        {
            var label = new Label { Text = value, TextColor = color, Wrap = WrapMode.Word,
                TextAlignment = TextAlignment.Left };
            label.LoadComplete += (_, _) => { label.TextAlignment = TextAlignment.Right; label.TextAlignment = TextAlignment.Left; };
            labels.Add(label);
            return label;
        }
        var symbol = title.Contains("question", StringComparison.OrdinalIgnoreCase) ? "?" :
            title.Contains("captur", StringComparison.OrdinalIgnoreCase) ? "▣" :
            state == "Finished" ? "✓" : state == "Running" ? "◌" : "›";
        body.Items.Add(Text(symbol + "  " + title + (string.IsNullOrWhiteSpace(state) ? "" : " · " + state), FoundryTheme.SecondaryText));
        if (!string.IsNullOrWhiteSpace(detail))
            body.Items.Add(Text(detail, FoundryTheme.PrimaryText));
        var queued = false;
        var lastWidth = 0;
        void FitText()
        {
            if (queued || IsDisposed) return;
            queued = true;
            Application.Instance.AsyncInvoke(() =>
            {
                queued = false;
                if (IsDisposed) return;
                var width = Math.Max(80, body.ClientSize.Width - FoundryTheme.Space2 * 2);
                if (width == lastWidth) return;
                lastWidth = width;
                foreach (var label in labels) label.Width = width;
            });
        }
        body.SizeChanged += (_, _) => FitText();
        body.LoadComplete += (_, _) => FitText();
        if (image is not null)
        {
            var preview = new FoundryThumbnailGallery { ToolTip = "Inspect image — click or press Enter",
                BackgroundColor = FoundryTheme.PanelBackground };
            preview.SetItems([new FoundryThumbnailItem("Inspect image", image)]);
            preview.SetLayout(280, 280);
            preview.SelectionChanged += (_, _) =>
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
                preview.SetSelectedName(null);
            };
            body.Items.Add(FoundryPreflightSummary.Left(preview));
        }
        Content = body;
    }
}
