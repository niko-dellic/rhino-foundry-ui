using Eto.Drawing;
using Eto.Forms;

namespace RhinoFoundry.UI;

/// <summary>Product-neutral destructive confirmation overlay. Consumers supply all operation wording.</summary>
public class FoundryConfirmationOverlay : Panel
{
    private readonly Panel _card;
    private readonly FoundryThemeBinding _themeBinding;
    private readonly Label _title;
    private readonly Label _message;
    private readonly Label _consequence;
    private readonly FoundryDialogButton _cancelButton;
    private readonly FoundryDialogButton _deleteButton;
    private bool _busy;

    public FoundryConfirmationOverlay()
    {
        BackgroundColor = FoundryTheme.WithAlpha(Colors.Black, FoundryTheme.IsDarkMode ? 150 : 105);
        Visible = false;

        _title = new Label
        {
            Text = string.Empty,
            Font = SystemFonts.Bold(14),
            TextColor = FoundryTheme.PrimaryText,
            TextAlignment = TextAlignment.Left,
            Wrap = WrapMode.Word,
        };
        _message = new Label
        {
            TextColor = FoundryTheme.PrimaryText,
            TextAlignment = TextAlignment.Left,
            Wrap = WrapMode.Word,
        };
        _consequence = new Label
        {
            Font = SystemFonts.Default(9),
            TextColor = FoundryTheme.MutedText,
            TextAlignment = TextAlignment.Left,
            Wrap = WrapMode.Word,
        };
        _cancelButton = new FoundryDialogButton(
            "Cancel",
            FoundryDialogButtonStyle.Secondary);
        _deleteButton = new FoundryDialogButton(
            "Confirm",
            FoundryDialogButtonStyle.Destructive);

        _cancelButton.Click += (_, _) => CancelRequested?.Invoke(this, EventArgs.Empty);
        _deleteButton.Click += (_, _) => ConfirmRequested?.Invoke(this, EventArgs.Empty);
        _cancelButton.KeyDown += OnActionKeyDown;
        _deleteButton.KeyDown += OnActionKeyDown;
        KeyDown += OnActionKeyDown;
        MouseDown += (_, eventArgs) => eventArgs.Handled = true;

        var warningIcon = new DeleteWarningIcon();
        var cardContent = new StackLayout
        {
            Padding = new Padding(FoundryTheme.Space4 + FoundryTheme.Space1),
            Spacing = FoundryTheme.Space4,
            HorizontalContentAlignment = HorizontalAlignment.Stretch,
            Items =
            {
                new StackLayout
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = FoundryTheme.Space3,
                    VerticalContentAlignment = VerticalAlignment.Top,
                    Items =
                    {
                        warningIcon,
                        new StackLayoutItem(new StackLayout
                        {
                            Spacing = FoundryTheme.Space1,
                            HorizontalContentAlignment = HorizontalAlignment.Stretch,
                            Items =
                            {
                                _title,
                                _message,
                                _consequence,
                            },
                        }, expand: true),
                    },
                },
                new StackLayout
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = FoundryTheme.Space2,
                    HorizontalContentAlignment = HorizontalAlignment.Right,
                    Items =
                    {
                        new StackLayoutItem(null, expand: true),
                        _cancelButton,
                        _deleteButton,
                    },
                },
            },
        };
        var cardSurface = new Panel
        {
            BackgroundColor = FoundryTheme.CanvasSurface,
            Content = cardContent,
        };
        _card = new Panel
        {
            Padding = new Padding(1),
            BackgroundColor = FoundryTheme.CanvasBorder,
            Content = cardSurface,
        };

        Content = new TableLayout
        {
            Padding = new Padding(FoundryTheme.Space4),
            Rows =
            {
                new TableRow { ScaleHeight = true },
                new TableRow(
                    new TableCell(null, scaleWidth: true),
                    new TableCell(_card),
                    new TableCell(null, scaleWidth: true)),
                new TableRow { ScaleHeight = true },
            },
        };
        SizeChanged += (_, _) => UpdateCardWidth();
        _themeBinding = new FoundryThemeBinding(this, () =>
        {
            BackgroundColor = FoundryTheme.WithAlpha(Colors.Black, FoundryTheme.IsDarkMode ? 150 : 105);
            _card.BackgroundColor = FoundryTheme.CanvasBorder;
            cardSurface.BackgroundColor = FoundryTheme.CanvasSurface;
            _title.TextColor = _message.TextColor = FoundryTheme.PrimaryText;
            _consequence.TextColor = FoundryTheme.MutedText;
            Invalidate(true);
        });
    }

    protected override void Dispose(bool disposing)
    {
        if (IsDisposed) return;
        if (disposing) _themeBinding.Dispose();
        base.Dispose(disposing);
    }

    public event EventHandler? CancelRequested;

    public event EventHandler? ConfirmRequested;

    public void ShowConfirmation(string title, string message, string consequence, string confirmText = "Confirm")
    {
        _busy = false;
        _title.Text = title;
        _message.Text = message;
        _consequence.Text = consequence;
        _consequence.Visible = true;
        _cancelButton.Text = "Cancel";
        _deleteButton.Text = confirmText;
        _cancelButton.Enabled = true;
        _deleteButton.Enabled = true;
        Visible = true;
        UpdateCardWidth();
        Application.Instance.AsyncInvoke(() =>
        {
            if (!IsDisposed && Visible && !_busy)
                _cancelButton.Focus();
        });
    }

    public void ShowBusy(string title, string message, string busyText = "Working…")
    {
        _busy = true;
        _title.Text = title;
        _message.Text = message;
        _consequence.Visible = false;
        _deleteButton.Text = busyText;
        _cancelButton.Enabled = false;
        _deleteButton.Enabled = false;
    }

    public void Dismiss()
    {
        Visible = false;
        _busy = false;
        _cancelButton.Enabled = true;
        _deleteButton.Enabled = true;
        _deleteButton.Text = "Confirm";
    }

    private void OnActionKeyDown(object? sender, KeyEventArgs eventArgs)
    {
        if (eventArgs.Key != Keys.Escape || _busy)
            return;

        eventArgs.Handled = true;
        CancelRequested?.Invoke(this, EventArgs.Empty);
    }

    private void UpdateCardWidth()
    {
        var availableWidth = Math.Max(0, ClientSize.Width - FoundryTheme.Space4 * 2);
        _card.Width = Math.Min(400, availableWidth);
    }

    private sealed class DeleteWarningIcon : Drawable
    {
        internal DeleteWarningIcon()
            : base(true)
        {
            Size = new Size(36, 36);
            BackgroundColor = Colors.Transparent;
            Paint += OnPaint;
        }

        private static void OnPaint(object? sender, PaintEventArgs eventArgs)
        {
            var graphics = eventArgs.Graphics;
            graphics.AntiAlias = true;
            graphics.FillEllipse(
                FoundryTheme.WithAlpha(FoundryTheme.DangerAccent, 34),
                0,
                0,
                36,
                36);
            using var pen = new Pen(FoundryTheme.DangerAccent, 1.25f);
            graphics.DrawLine(pen, 11, 13, 25, 13);
            graphics.DrawLine(pen, 14, 10, 22, 10);
            graphics.DrawLine(pen, 16, 8, 20, 8);
            graphics.DrawRectangle(pen, 13, 13, 10, 14);
            graphics.DrawLine(pen, 16.5f, 16, 16.5f, 24);
            graphics.DrawLine(pen, 19.5f, 16, 19.5f, 24);
        }
    }
}
