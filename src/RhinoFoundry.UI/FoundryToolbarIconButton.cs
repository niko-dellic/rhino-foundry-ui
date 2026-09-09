using Eto.Drawing;
using Eto.Forms;

namespace RhinoFoundry.UI;

/// <summary>
/// Shared quiet-outline icon button used by Foundry toolbars. Active toggles
/// use a neutral surface instead of a separate accent underline.
/// </summary>
public sealed class FoundryToolbarIconButton : Drawable
{
    /// <summary>Circular high-contrast composer action. Supply an icon in PanelBackground color.</summary>
    public bool IsComposerAction { get; init; }
    private readonly bool _isToggle;
    private Image _image;
    private bool _checked;
    private bool _hovered;
    private bool _pressed;
    private bool _showFocusRing;
    private bool _isGrouped;

    public FoundryToolbarIconButton(
        Image image,
        string toolTip,
        bool isToggle = false)
        : base(true)
    {
        _image = image ?? throw new ArgumentNullException(nameof(image));
        _isToggle = isToggle;
        Size = new Size(32, 32);
        BackgroundColor = Colors.Transparent;
        CanFocus = true;
        ToolTip = toolTip;

        Paint += OnPaint;
        MouseEnter += (_, _) =>
        {
            if (!Enabled) return;
            _hovered = true;
            Invalidate();
        };
        MouseLeave += (_, _) =>
        {
            _hovered = false;
            _pressed = false;
            Invalidate();
        };
        MouseDown += (_, eventArgs) =>
        {
            if (!Enabled || !eventArgs.Buttons.HasFlag(MouseButtons.Primary)) return;
            _pressed = true;
            _showFocusRing = false;
            Focus();
            eventArgs.Handled = true;
            Invalidate();
        };
        MouseUp += (_, eventArgs) =>
        {
            if (!Enabled || !_pressed) return;
            _pressed = false;
            Activate();
            eventArgs.Handled = true;
        };
        KeyDown += (_, eventArgs) =>
        {
            if (!Enabled || eventArgs.Key is not (Keys.Enter or Keys.Space)) return;
            Activate();
            eventArgs.Handled = true;
        };
        GotFocus += (_, _) =>
        {
            if (!_pressed) _showFocusRing = true;
            Invalidate();
        };
        LostFocus += (_, _) =>
        {
            _pressed = false;
            _showFocusRing = false;
            Invalidate();
        };
        EnabledChanged += (_, _) =>
        {
            if (!Enabled)
            {
                _hovered = false;
                _pressed = false;
                _showFocusRing = false;
            }

            Invalidate();
        };
    }

    public event EventHandler? Click;

    public Image Image
    {
        get => _image;
        set
        {
            _image = value ?? throw new ArgumentNullException(nameof(value));
            Invalidate();
        }
    }

    public bool Checked
    {
        get => _checked;
        set
        {
            if (_checked == value) return;
            _checked = value;
            Invalidate();
        }
    }

    public bool IsGrouped
    {
        get => _isGrouped;
        set
        {
            if (_isGrouped == value) return;
            _isGrouped = value;
            Invalidate();
        }
    }

    private void Activate()
    {
        if (_isToggle) Checked = !Checked;
        Click?.Invoke(this, EventArgs.Empty);
        Invalidate();
    }

    private void OnPaint(object? sender, PaintEventArgs eventArgs)
    {
        var bounds = new RectangleF(0.5f, 0.5f, Math.Max(0, Width - 1), Math.Max(0, Height - 1));
        using var outline = GraphicsPath.GetRoundRect(bounds, IsComposerAction ? 16 : 6);
        if (IsComposerAction)
        {
            eventArgs.Graphics.FillPath(Enabled ? FoundryTheme.PrimaryText : FoundryTheme.MutedText, outline);
        }
        else if (Checked)
        {
            eventArgs.Graphics.FillPath(FoundryTheme.ToolbarActiveBackground, outline);
        }
        else if (!IsGrouped)
        {
            eventArgs.Graphics.FillPath(FoundryTheme.ToolbarButtonBackground, outline);
        }

        if (!Checked && Enabled && (_hovered || _pressed))
        {
            eventArgs.Graphics.FillPath(
                FoundryTheme.WithAlpha(FoundryTheme.CanvasSubtleSurface, _pressed ? 205 : 135),
                outline);
        }
        if (!IsGrouped || Checked)
        {
            eventArgs.Graphics.DrawPath(
                new Pen(
                    Checked
                        ? FoundryTheme.WithAlpha(FoundryTheme.SecondaryText, Enabled ? 215 : 80)
                        : FoundryTheme.WithAlpha(FoundryTheme.CanvasBorder, Enabled ? 175 : 70),
                    1),
                outline);
        }

        var imageSize = _image.Size;
        var imageX = (Width - imageSize.Width) / 2f;
        var imageY = (Height - imageSize.Height) / 2f;
        eventArgs.Graphics.DrawImage(_image, imageX, imageY);

        if (!Enabled && !IsComposerAction)
        {
            eventArgs.Graphics.FillRectangle(
                FoundryTheme.WithAlpha(FoundryTheme.PanelBackground, 155),
                imageX,
                imageY,
                imageSize.Width,
                imageSize.Height);
        }

        if (Enabled && HasFocus && _showFocusRing)
        {
            using var focus = GraphicsPath.GetRoundRect(
                new RectangleF(2.5f, 2.5f, Math.Max(0, Width - 5), Math.Max(0, Height - 5)),
                4);
            eventArgs.Graphics.DrawPath(
                new Pen(FoundryTheme.WithAlpha(FoundryTheme.PrimaryText, 155), 1),
                focus);
        }
    }
}
