using Eto.Drawing;
using Eto.Forms;
namespace RhinoFoundry.UI;
/// <summary>Vertical viewport that sizes content without recursive AppKit layout notifications.</summary>
public sealed class FoundryScrollable : Scrollable
{
    private int _appliedWidth;
    private bool _queued;
    public FoundryScrollable(Control content)
    {
        Content = content; Border = BorderType.None;
        ExpandContentWidth = false; ExpandContentHeight = true;
        SizeChanged += (_, _) => RequestWidthSync();
        LoadComplete += (_, _) => RequestWidthSync();
    }
    public void RequestWidthSync()
    {
        if (_queued || IsDisposed) return;
        _queued = true;
        Application.Instance.AsyncInvoke(() =>
        {
            _queued = false;
            if (IsDisposed || Content is null) return;
            var width = ClientSize.Width;
            if (width <= 1 || width == _appliedWidth) return;
            _appliedWidth = width;
            if (Content is Panel panel) panel.MinimumSize = new Size(width, 0);
            Content.Width = width;
        });
    }
}
