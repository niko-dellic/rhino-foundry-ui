using Eto.Forms;

namespace RhinoFoundry.UI;

public enum FoundryViewMode
{
    Table,
    Thumbnails,
    Canvas,
}

public sealed class FoundryViewModeChangedEventArgs(FoundryViewMode mode) : EventArgs
{
    public FoundryViewMode Mode { get; } = mode;
}

/// <summary>
/// Keyboard-accessible segmented selector for table, thumbnail, and canvas views.
/// </summary>
public sealed class FoundryViewModeSelector : Panel
{
    private readonly FoundryToolbarIconButton _table;
    private readonly FoundryToolbarIconButton _thumbnails;
    private readonly FoundryToolbarIconButton _canvas;
    private FoundryViewMode _selectedMode;

    public FoundryViewModeSelector(FoundryViewMode selectedMode = FoundryViewMode.Table)
    {
        _table = new FoundryToolbarIconButton(FoundryViewIcons.Table(), "Table view", true);
        _thumbnails = new FoundryToolbarIconButton(
            FoundryViewIcons.Thumbnails(),
            "Thumbnail view",
            true);
        _canvas = new FoundryToolbarIconButton(FoundryViewIcons.Canvas(), "Canvas view", true);
        _table.Click += (_, _) => SelectedMode = FoundryViewMode.Table;
        _thumbnails.Click += (_, _) => SelectedMode = FoundryViewMode.Thumbnails;
        _canvas.Click += (_, _) => SelectedMode = FoundryViewMode.Canvas;
        Content = new FoundryToolbarButtonGroup(_table, _thumbnails, _canvas);
        SelectedMode = selectedMode;
    }

    public event EventHandler<FoundryViewModeChangedEventArgs>? SelectedModeChanged;

    public FoundryViewMode SelectedMode
    {
        get => _selectedMode;
        set
        {
            var changed = _selectedMode != value;
            _selectedMode = value;
            _table.Checked = value == FoundryViewMode.Table;
            _thumbnails.Checked = value == FoundryViewMode.Thumbnails;
            _canvas.Checked = value == FoundryViewMode.Canvas;
            if (changed)
                SelectedModeChanged?.Invoke(this, new FoundryViewModeChangedEventArgs(value));
        }
    }
}
