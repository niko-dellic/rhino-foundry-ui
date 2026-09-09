using Eto.Drawing;
using Eto.Forms;

namespace RhinoFoundry.UI;

/// <summary>A wrapping, width-limited message. Outgoing messages use the composer surface and align right.</summary>
public sealed class FoundryChatMessage : PixelLayout
{
    /// <summary>Creates a text-only message; owns its label and surface. Text is never interpreted as markup.</summary>
    public FoundryChatMessage(string text, bool outgoing)
    {
        var label = new Label { Text = text, Wrap = WrapMode.Word, TextAlignment = TextAlignment.Left,
            TextColor = FoundryTheme.PrimaryText, BackgroundColor = Colors.Transparent };
        label.LoadComplete += (_, _) => { label.TextAlignment = TextAlignment.Right; label.TextAlignment = TextAlignment.Left; };
        Control content = outgoing
            ? new FoundryFormField(new Panel { Padding = new Padding(0, FoundryTheme.Space3), Content = label }, 48, horizontalInset: FoundryTheme.Space4, cornerRadius: 16)
            : label;
        Add(content, 0, 0);
        var queued = false;
        var lastWidth = -1;
        void Fit()
        {
            if (queued || IsDisposed) return;
            queued = true;
            Application.Instance.AsyncInvoke(() =>
            {
                queued = false;
                if (IsDisposed || ClientSize.Width <= 0 || lastWidth == ClientSize.Width) return;
                lastWidth = ClientSize.Width;
                var width = Math.Min(760, Math.Max(100, (int)(ClientSize.Width * .86)));
                var inset = outgoing ? FoundryTheme.Space4 * 2 : 0;
                // Clear the previous explicit height before measuring at the new width.
                // Incoming messages use the label itself as their sized content.
                label.Height = -1;
                label.Width = Math.Max(60, width - inset);
                var height = Math.Max(24, (int)Math.Ceiling(label.GetPreferredSize(new Size(label.Width, -1)).Height));
                content.Size = new Size(width, height + (outgoing ? FoundryTheme.Space4 * 2 : 0));
                Height = content.Height;
                Move(content, outgoing ? Math.Max(0, ClientSize.Width - width) : 0, 0);
            });
        }
        SizeChanged += (_, _) => Fit();
        LoadComplete += (_, _) => Fit();
    }
}

/// <summary>Rounded multiline composer. The caller owns sending/cancellation; native editing and Tab order are retained.</summary>
public sealed class FoundryChatComposer : Panel
{
    public FoundryChatComposer(TextArea editor, Control send, Control stop)
    {
        ArgumentNullException.ThrowIfNull(editor);
        editor.BackgroundColor = FoundryTheme.InputBackground;
        editor.TextColor = FoundryTheme.PrimaryText;
        editor.Height = 72;
        editor.LoadComplete += (_, _) =>
        {
            // The outer Foundry shell owns the border; retain the native text editor and accessibility.
            try
            {
                var native = editor.ControlObject;
                var scroll = native?.GetType().GetProperty("EnclosingScrollView")?.GetValue(native) ?? native;
                var border = scroll?.GetType().GetProperty("BorderType");
                if (border is { CanWrite: true } && border.PropertyType.IsEnum)
                    border.SetValue(scroll, Enum.ToObject(border.PropertyType, 0));
            }
            catch (Exception) { /* Keep the portable native editor when a handler has no border adapter. */ }
        };
        var content = new StackLayout { Padding = new Padding(0, FoundryTheme.Space2),
            HorizontalContentAlignment = HorizontalAlignment.Stretch, Spacing = FoundryTheme.Space2,
            Items = { editor, new StackLayout { Orientation = Orientation.Horizontal,
                Items = { new StackLayoutItem(null, true), send, stop } } } };
        Content = new FoundryFormField(content, 128, editor, horizontalInset: FoundryTheme.Space3, cornerRadius: 16);
    }
}
