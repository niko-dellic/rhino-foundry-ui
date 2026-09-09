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
                var desired = Math.Clamp((int)Math.Ceiling(label.GetPreferredSize(new Size(Math.Max(40, ClientSize.Width - 20), -1)).Height) + 8, 24, maximumHeight - 8);
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
