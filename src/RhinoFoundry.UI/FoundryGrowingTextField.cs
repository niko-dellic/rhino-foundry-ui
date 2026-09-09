using Eto.Drawing;
using Eto.Forms;

namespace RhinoFoundry.UI;

/// <summary>One-line minimum editor, growing with wrapping to a bounded height. Owns its supplied editor.</summary>
public sealed class FoundryGrowingTextField : Panel
{
    public FoundryGrowingTextField(TextArea editor, int maximumHeight = 200, bool showBorder = true, string placeholder = "")
    {
        if (maximumHeight < 32) throw new ArgumentOutOfRangeException(nameof(maximumHeight));
        editor.Wrap = true;
        editor.Height = 24;
        editor.LoadComplete += (_, _) =>
        {
            // The shared shell, not AppKit's scroll view, owns the border.
            if (!OperatingSystem.IsMacOS()) return;
            try
            {
                var native = editor.ControlObject;
                var scroll = native?.GetType().GetProperty("EnclosingScrollView")?.GetValue(native) ?? native;
                var border = scroll?.GetType().GetProperty("BorderType");
                if (border is { CanWrite: true } && border.PropertyType.IsEnum)
                    border.SetValue(scroll, Enum.ToObject(border.PropertyType, 0));
            }
            catch (Exception) { /* Preserve native editing if the platform adapter is unavailable. */ }
        };
        Control field = showBorder ? new FoundryFormField(editor, 32, editor) : editor;
        var overlay = new PixelLayout();
        var hint = new Label { Text = placeholder, TextColor = FoundryTheme.MutedText, TextAlignment = TextAlignment.Left };
        hint.MouseDown += (_, _) => editor.Focus();
        hint.LoadComplete += (_, _) => { hint.TextAlignment = TextAlignment.Right; hint.TextAlignment = TextAlignment.Left; };
        overlay.Add(field, 0, 0);
        overlay.Add(hint, 12, 6);
        Content = overlay;
        var queued = false;
        void Fit()
        {
            if (queued || IsDisposed) return;
            queued = true;
            Application.Instance.AsyncInvoke(() =>
            {
                queued = false;
                if (IsDisposed || ClientSize.Width <= 0) return;
                using var label = new Label { Text = string.IsNullOrEmpty(editor.Text) ? " " : editor.Text + " ", Font = editor.Font, Wrap = WrapMode.Word };
                var desired = Math.Clamp((int)Math.Ceiling(label.GetPreferredSize(new Size(Math.Max(40, ClientSize.Width - 20), 100000)).Height) + 8, 24, maximumHeight - 8);
                if (editor.Height != desired) editor.Height = desired;
                Height = desired + 8;
                field.Size = new Size(Math.Max(40, ClientSize.Width), Height);
                hint.Width = Math.Max(24, ClientSize.Width - 24);
                hint.Visible = !string.IsNullOrEmpty(placeholder) && string.IsNullOrEmpty(editor.Text);
            });
        }
        editor.TextChanged += (_, _) => Fit();
        SizeChanged += (_, _) => Fit();
        LoadComplete += (_, _) => Fit();
    }
}
