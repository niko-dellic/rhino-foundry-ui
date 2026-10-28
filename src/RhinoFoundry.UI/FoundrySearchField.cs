using Eto.Drawing;
using Eto.Forms;

namespace RhinoFoundry.UI;

/// <summary>
/// Search input with Foundry field chrome and an embedded search icon.
/// </summary>
public sealed class FoundrySearchField : Panel
{
    private readonly TextBox _textBox;
    private readonly FoundryToolbarField _field;
    private readonly ImageView _icon;
    private readonly FoundryThemeBinding _themeBinding;

    public FoundrySearchField(
        string placeholder = "Search",
        string? toolTip = null,
        int width = 260)
    {
        _textBox = new TextBox
        {
            PlaceholderText = placeholder,
            ToolTip = toolTip,
            ShowBorder = false,
            BackgroundColor = Colors.Transparent,
        };
        _icon = new ImageView { Image = FoundryViewIcons.Search(), Size = new Size(18, 18) };
        var input = new StackLayout
        {
            Orientation = Orientation.Horizontal,
            Spacing = FoundryTheme.Space2,
            VerticalContentAlignment = VerticalAlignment.Center,
            Items =
            {
                _icon,
                new StackLayoutItem(_textBox, true),
            },
        };
        _field = new FoundryToolbarField(input, width, _textBox);
        Content = _field;
        Width = width;
        Height = 32;
        _themeBinding = new FoundryThemeBinding(this, () =>
        {
            var previous = _icon.Image;
            _icon.Image = FoundryViewIcons.Search();
            previous?.Dispose();
        });
    }

    protected override void Dispose(bool disposing)
    {
        if (IsDisposed) return;
        if (disposing && !IsDisposed)
        {
            _themeBinding.Dispose();
            _icon.Image?.Dispose();
        }
        base.Dispose(disposing);
    }

    public event EventHandler<EventArgs>? TextChanged
    {
        add => _textBox.TextChanged += value;
        remove => _textBox.TextChanged -= value;
    }

    public string Text
    {
        get => _textBox.Text;
        set => _textBox.Text = value ?? string.Empty;
    }

    public string PlaceholderText
    {
        get => _textBox.PlaceholderText;
        set => _textBox.PlaceholderText = value ?? string.Empty;
    }

    public TextBox Input => _textBox;

    public int FieldWidth
    {
        get => Width;
        set
        {
            Width = value;
            _field.Width = value;
        }
    }
}
