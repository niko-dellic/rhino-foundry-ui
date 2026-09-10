using Eto.Drawing;
using Eto.Forms;

namespace RhinoFoundry.UI;

/// <summary>
/// Single-line click-to-rename title. Owns its editors, not persistence.
/// Value assignment is silent and cancels an active edit. User commits trim whitespace;
/// empty edits are cancelled. Enter commits, Escape cancels, and focus loss commits.
/// </summary>
public sealed class FoundryEditableTitle : Panel
{
    private readonly FoundryDialogButton _button;
    private readonly TextBox _editor = new();
    private readonly FoundryFormField _field;
    private readonly Font _font = SystemFonts.Bold(13);
    private string _value;
    private bool _editing;
    private bool _disposing;

    /// <param name="value">Nonempty initial title.</param>
    /// <param name="width">Width in logical pixels, at least 80.</param>
    public FoundryEditableTitle(string value, int width = 280)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("A title is required.", nameof(value));
        if (width < 80) throw new ArgumentOutOfRangeException(nameof(width));
        _value = value.Trim();
        Width = width;
        Height = 32;
        _button = new FoundryDialogButton(_value, FoundryDialogButtonStyle.Heading, width) { LeftAlignText = true };
        _editor.Font = _font;
        _field = new FoundryFormField(_editor, minimumHeight: 32, fixedHeight: 32);
        _button.Click += (_, _) => BeginEdit();
        _editor.KeyDown += (_, e) =>
        {
            if (e.Key == Keys.Enter) { e.Handled = true; Finish(true, true); }
            else if (e.Key == Keys.Escape) { e.Handled = true; Finish(false, true); }
        };
        _editor.LostFocus += (_, _) => Finish(true, false);
        EnabledChanged += (_, _) => { if (!Enabled) Finish(false, false); };
        UnLoad += (_, _) => Finish(false, false);
        SizeChanged += (_, _) => RefreshCaption();
        Content = _button;
        RefreshCaption();
    }

    /// <summary>Raised once after a nonempty user commit, even if the value is unchanged.</summary>
    public event EventHandler? Committed;

    /// <summary>Full title; programmatic assignment never raises Committed.</summary>
    public string Value
    {
        get => _value;
        set
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("A title is required.", nameof(value));
            Finish(false, false);
            _value = value.Trim();
            RefreshCaption();
        }
    }

    /// <summary>Whether the inline editor is open.</summary>
    public bool IsEditing => _editing;

    /// <summary>Begins editing when enabled. Repeated calls are ignored.</summary>
    public void BeginEdit()
    {
        if (!Enabled || _editing || _disposing || IsDisposed) return;
        _editor.Text = _value;
        _editing = true;
        Content = _field;
        _editor.Focus();
        _editor.SelectAll();
    }

    private void Finish(bool commit, bool focus)
    {
        if (!_editing || _disposing) return;
        _editing = false; // Detaching the editor can synchronously raise LostFocus.
        var value = _editor.Text.Trim();
        var accepted = commit && Enabled && value.Length > 0;
        if (accepted) _value = value;
        Content = _button;
        RefreshCaption();
        if (focus && Enabled) _button.Focus();
        if (accepted) Committed?.Invoke(this, EventArgs.Empty);
    }

    private void RefreshCaption()
    {
        if (_disposing || IsDisposed) return;
        var available = Math.Max(0, Width - FoundryTheme.Space3 * 2);
        var text = _value;
        if (_font.MeasureString(text).Width > available)
        {
            while (text.Length > 0 && _font.MeasureString(text + "…").Width > available)
                text = text[..^1];
            text += "…";
        }
        _button.Text = text;
        _button.ToolTip = "Rename: " + _value;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing && !_disposing)
        {
            _disposing = true;
            Content = null;
            _field.Dispose();
            _button.Dispose();
            _font.Dispose();
        }
        base.Dispose(disposing);
    }
}
