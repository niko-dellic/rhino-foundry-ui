using Eto.Drawing;
using Eto.Forms;

using RhinoFoundry.UI;

namespace RhinoFoundry.UI;

/// <summary>
/// Width-constrained page without feeding inset editors' preferred widths back
/// into the dock's minimum size. Native layout must settle before measuring it.
/// </summary>
public sealed class FoundryResponsivePage : Scrollable
{
    /// <summary>Reserved width for native scrollbar chrome, in logical pixels.</summary>
    public int ScrollbarReserve { get; init; } = 20;
    private readonly StackLayout _column;
    private readonly PixelLayout _canvas;
    private readonly StackLayoutItem[] _sections;
    private bool _queued, _applying, _resetWidths;
    private int _fittedWidth = -1;
    private int _generation;

    public FoundryResponsivePage(params Control[] sections)
    {
        Border = BorderType.None;
        ExpandContentWidth = false;
        ExpandContentHeight = true;
        _column = new StackLayout
        {
            Spacing = FoundryTheme.Space4, AlignLabels = false,
            HorizontalContentAlignment = HorizontalAlignment.Stretch,
            VerticalContentAlignment = VerticalAlignment.Top,
        };
        _sections = sections.Select(section => new StackLayoutItem(section, HorizontalAlignment.Stretch)).ToArray();
        SynchronizeSections();
        _canvas = new PixelLayout();
        _canvas.Add(_column, 0, 0);
        Content = _canvas;
        SizeChanged += (_, _) => RequestFit();
        LoadComplete += (_, _) => Activate();
        UnLoad += (_, _) => { _generation++; _queued = false; };
        foreach (var section in sections) WatchContent(section);
    }

    public void ApplyTheme(Color? background = null)
    {
        BackgroundColor = _canvas.BackgroundColor = _column.BackgroundColor = background ?? FoundryTheme.PanelBackground;
        Invalidate();
    }

    public void Activate() { SynchronizeSections(); RequestFit(resetWidths: true); }
    public void RefreshContent() { SynchronizeSections(); RequestFit(); }

    private void SynchronizeSections()
    {
        // Preserve visible section order without leaving gaps for hidden sections.
        int position = 0;
        foreach (var section in _sections)
        {
            if (!section.Control.Visible) _column.Items.Remove(section);
            else
            {
                if (!_column.Items.Contains(section)) _column.Items.Insert(position, section);
                position++;
            }
        }
    }

    private void WatchContent(Control control)
    {
        // Expanding sections and wrapping status text can change the scroll
        // extent without changing the dock's width.
        if (control is Expander expander)
            expander.ExpandedChanged += (_, _) => RequestFit();
        if (control is Label label)
            label.TextChanged += (_, _) => RequestFit();
        if (control is Container container)
            foreach (var child in container.Controls) WatchContent(child);
    }

    private void RequestFit(bool resetWidths = false)
    {
        _resetWidths |= resetWidths;
        if (IsDisposed || !Loaded || _queued || _applying) return;
        _queued = true;
        var generation = _generation;
        Application.Instance.AsyncInvoke(() =>
        {
            if (IsDisposed || generation != _generation) return;
            _queued = false;
            if (!Loaded) return;
            Fit(generation);
        });
    }

    private void Fit(int generation)
    {
        var width = ClientSize.Width - ScrollbarReserve;
        // An unattached/initializing page has no usable native allocation yet.
        // Never cache a fabricated 1px width as a completed layout.
        if (width <= 0 || ClientSize.Height <= 0) return;
        _applying = true;
        try
        {
            if (_resetWidths || width != _fittedWidth)
            {
                ReleaseFieldWidths(_column);
                _fittedWidth = width;
                _resetWidths = false;
            }
            _canvas.Width = width;
            // Allocate width BEFORE asking nested rows for their preferred height.
            _column.Width = width;
            MeasureHeight(width);
        }
        finally { _applying = false; }

        // AppKit may defer child constraints until the next UI turn. Perform one
        // bounded settling pass, without resetting widths/restarting the cycle.
        Application.Instance.AsyncInvoke(() =>
        {
            if (IsDisposed || generation != _generation || !Loaded || ClientSize.Width - ScrollbarReserve != width) return;
            _applying = true;
            try { MeasureHeight(width); }
            finally { _applying = false; }
        });
    }

    private void MeasureHeight(int width)
    {
        FitRows(_column, width);
        // A previous explicit height must not constrain the next measurement.
        _column.Height = -1;
        var height = Math.Max(1, (int)Math.Ceiling(_column.GetPreferredSize(new SizeF(width, -1)).Height));
        _column.Size = new Size(width, height);
        _canvas.Size = new Size(width, Math.Max(ClientSize.Height, height));
    }

    private static void FitRows(Control control, int width)
    {
        if (control is FoundryResponsiveRow row) row.FitWidth(width);
        else if (control is Container container)
            foreach (var child in container.Controls) FitRows(child, width);
    }

    private static void ReleaseFieldWidths(Control control)
    {
        if (control is FoundryFormField or FoundryInsetFormField)
        {
            control.Width = 0;
            control.Width = -1;
        }
        else if (control is Container container)
            foreach (var child in container.Controls) ReleaseFieldWidths(child);
    }
}
