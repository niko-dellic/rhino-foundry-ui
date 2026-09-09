using Eto.Drawing;
using Eto.Forms;
namespace RhinoFoundry.UI;

/// <summary>Read-only preflight facts with wrapping badges matching the multi-select control.</summary>
public sealed class FoundryPreflightSummary : Panel
{
    private string[] _badges = [];
    private KeyValuePair<string, string>[] _facts = [];
    private bool _queued;
    private int _width;
    public FoundryPreflightSummary()
    {
        SizeChanged += (_, _) => QueueLayout();
        LoadComplete += (_, _) => QueueLayout();
    }
    /// <summary>Copies presentation data. Replaces and disposes owned content; raises no events.</summary>
    public void SetSummary(IEnumerable<string> badges, IEnumerable<KeyValuePair<string, string>> facts)
    {
        ArgumentNullException.ThrowIfNull(badges);
        ArgumentNullException.ThrowIfNull(facts);
        _badges = badges.ToArray(); _facts = facts.ToArray(); Render();
    }
    private void QueueLayout()
    {
        if (_queued || IsDisposed) return;
        _queued = true;
        Application.Instance.AsyncInvoke(() =>
        {
            _queued = false;
            if (!IsDisposed && ClientSize.Width > 0 && ClientSize.Width != _width) Render();
        });
    }
    private void Render()
    {
        _width = ClientSize.Width;
        var available = Math.Max(120, _width > 0 ? _width : 360);
        var body = new StackLayout { Spacing = FoundryTheme.Space3,
            HorizontalContentAlignment = HorizontalAlignment.Stretch };
        var rows = new StackLayout { Spacing = FoundryTheme.Space1,
            HorizontalContentAlignment = HorizontalAlignment.Stretch };
        StackLayout Row() => new() { Orientation = Orientation.Horizontal, Spacing = FoundryTheme.Space1 };
        var row = Row(); var used = 0;
        foreach (var text in _badges)
        {
            var badge = new FoundryRemovableBadge(text, removable: false);
            if (used > 0 && used + FoundryTheme.Space1 + badge.Width > available)
            {
                row.Items.Add(new StackLayoutItem(null, true)); rows.Items.Add(row); row = Row(); used = 0;
            }
            row.Items.Add(badge); used += badge.Width + FoundryTheme.Space1;
        }
        if (_badges.Length > 0)
        {
            row.Items.Add(new StackLayoutItem(null, true)); rows.Items.Add(row); body.Items.Add(rows);
        }
        foreach (var fact in _facts)
        {
            body.Items.Add(new StackLayout
            {
                Spacing = FoundryTheme.Space1, HorizontalContentAlignment = HorizontalAlignment.Stretch,
                Items = { Left(new Label { Text = fact.Key, TextColor = FoundryTheme.SecondaryText,
                    Font = FoundryTheme.BrandFont }), Left(new Label { Text = fact.Value,
                    TextColor = FoundryTheme.PrimaryText, Wrap = WrapMode.Word }) },
            });
        }
        var previous = Content; Content = body; previous?.Dispose();
    }
    /// <summary>Anchors natural-width text at the leading edge even when Rhino styles label alignment.</summary>
    public static Control Left(Control content) => new StackLayout
    {
        Orientation = Orientation.Horizontal,
        Items = { content, new StackLayoutItem(null, true) },
    };
}
