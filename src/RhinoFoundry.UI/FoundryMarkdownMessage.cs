using System.Text;
using Eto.Drawing;
using Eto.Forms;
using Markdig;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace RhinoFoundry.UI;

/// <summary>Content-only Markdown conversion. Raw HTML parsing is disabled.</summary>
public static class FoundryMarkdownContent
{
    internal static readonly MarkdownPipeline Pipeline = new MarkdownPipelineBuilder().UsePipeTables().DisableHtml().Build();
    public static string ToHtml(string markdown) => Markdown.ToHtml(markdown, Pipeline);
}

/// <summary>Native read-only Markdown. Owns its text controls; no browser, remote images or executable markup.</summary>
public sealed class FoundryMarkdownMessage : StackLayout
{
    public FoundryMarkdownMessage(string markdown)
    {
        HorizontalContentAlignment = HorizontalAlignment.Stretch;
        Spacing = FoundryTheme.Space2;
        foreach (var block in Markdown.Parse(markdown, FoundryMarkdownContent.Pipeline)) AddBlock(block);
    }

    private void AddBlock(Block block, string prefix = "")
    {
        if (block is Markdig.Extensions.Tables.Table table)
        {
            var rows = new List<string[]>();
            foreach (var row in table.OfType<Markdig.Extensions.Tables.TableRow>())
            {
                var cells = new List<string>();
                foreach (var cell in row.OfType<Markdig.Extensions.Tables.TableCell>())
                {
                    var text = new StringBuilder();
                    foreach (var leaf in cell.OfType<LeafBlock>()) text.Append(InlineText(leaf.Inline));
                    cells.Add(text.ToString());
                }
                rows.Add(cells.ToArray());
            }
            if (rows.Count > 0) Items.Add(new FoundryReadOnlyTable(rows[0], rows.Skip(1).ToArray()));
        }
        else if (block is ListBlock list)
        {
            var index = int.TryParse(list.OrderedStart, out var start) ? start : 1;
            foreach (var item in list.OfType<ListItemBlock>())
            {
                var first = true;
                foreach (var child in item)
                {
                    AddBlock(child, first ? list.IsOrdered ? $"{index}. " : "• " : "   ");
                    first = false;
                }
                index++;
            }
        }
        else if (block is LeafBlock leaf)
        {
            var rtf = Escape(prefix) + (leaf.Inline is null ? Escape(leaf.Lines.ToString()) : InlineRtf(leaf.Inline));
            Items.Add(Text(rtf, block is HeadingBlock, block is HeadingBlock heading ? Math.Max(14, 22 - heading.Level * 2) : 13));
        }
        else if (block is ContainerBlock container)
            foreach (var child in container) AddBlock(child, prefix);
    }

    private static string Escape(string value)
    {
        var result = new StringBuilder();
        foreach (var c in value)
            if (c is '\\' or '{' or '}') result.Append('\\').Append(c);
            else if (c == '\n') result.Append("\\line ");
            else if (c > 127) result.Append("\\u").Append((short)c).Append('?');
            else if (c != '\r') result.Append(c);
        return result.ToString();
    }

    private static string InlineText(ContainerInline? inline) => inline is null ? "" : string.Concat(inline.Select(child => child switch
    {
        LiteralInline literal => literal.Content.ToString(),
        CodeInline code => code.Content,
        LineBreakInline => " ",
        ContainerInline nested => InlineText(nested),
        _ => child.ToString() ?? "",
    }));

    private static string InlineRtf(ContainerInline? inline)
    {
        if (inline is null) return "";
        var result = new StringBuilder();
        foreach (var child in inline)
            result.Append(child switch
            {
                LiteralInline literal => Escape(literal.Content.ToString()),
                CodeInline code => "{\\f1 " + Escape(code.Content) + "}",
                LineBreakInline line => line.IsHard ? "\\line " : " ",
                EmphasisInline emphasis => "{" + (emphasis.DelimiterCount >= 2 ? "\\b " : "\\i ") + InlineRtf(emphasis) + "}",
                LinkInline link => InlineRtf(link),
                ContainerInline nested => InlineRtf(nested),
                _ => Escape(child.ToString() ?? ""),
            });
        return result.ToString();
    }

    private static Control Text(string content, bool bold, int size)
    {
        var color = FoundryTheme.PrimaryText;
        var editor = new RichTextArea { ReadOnly = true, Wrap = true, BackgroundColor = FoundryTheme.PanelBackground,
            TextColor = color, Font = new Font(SystemFont.Default, size), Height = 32 };
        editor.Rtf = "{\\rtf1\\ansi{\\fonttbl{\\f0 Helvetica;}{\\f1 Menlo;}}{\\colortbl;\\red" + color.Rb + "\\green" + color.Gb + "\\blue" + color.Bb + ";}\\cf1\\f0\\fs" + size * 2 + (bold ? "\\b " : " ") + content + "}";
        editor.MouseWheel += (_, e) =>
        {
            // Read-only prose belongs to the conversation, never to its native
            // paragraph editor's scrolling surface.
            Control? parent = editor.Parent;
            while (parent is not null && parent is not Scrollable) parent = parent.Parent;
            if (parent is Scrollable page)
            {
                var position = page.ScrollPosition;
                page.ScrollPosition = new Point(position.X, Math.Max(0, position.Y - (int)Math.Round(e.Delta.Height * 40)));
                e.Handled = true;
            }
        };
        var queued = false;
        var width = -1;
        void Fit()
        {
            if (queued || editor.IsDisposed) return;
            queued = true;
            Application.Instance.AsyncInvoke(() =>
            {
                queued = false;
                if (editor.IsDisposed || editor.Width <= 0 || width == editor.Width) return;
                width = editor.Width;
                using var measure = new Label { Text = editor.Text, Font = editor.Font, Wrap = WrapMode.Word };
                editor.Height = Math.Max(32, (int)Math.Ceiling(measure.GetPreferredSize(new Size(Math.Max(40, width - 24), -1)).Height * 1.25) + 16);
            });
        }
        editor.SizeChanged += (_, _) => Fit();
        editor.LoadComplete += (_, _) =>
        {
            // Read-only prose must not resemble an editable field. Preserve native selection.
            try
            {
                var native = editor.ControlObject;
                var scroll = native?.GetType().GetProperty("EnclosingScrollView")?.GetValue(native) ?? native;
                var border = scroll?.GetType().GetProperty("BorderType");
                if (border is { CanWrite: true } && border.PropertyType.IsEnum) border.SetValue(scroll, Enum.ToObject(border.PropertyType, 0));
            }
            catch (Exception) { /* Portable native text remains available. */ }
            Fit();
        };
        return editor;
    }
}
