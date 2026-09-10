using System.Reflection;
using System.Text.Json;
using Eto.Drawing;
using Eto.Forms;
using RhinoFoundry.UI.Primitives;

namespace RhinoFoundry.UI.HostChecks;

/// <summary>Run inside Rhino's initialized Eto UI thread. Does not read or change a document.</summary>
public static class ComponentChecks
{
    private static readonly MethodInfo KeyDown = typeof(Control).GetMethod("OnKeyDown", BindingFlags.Instance | BindingFlags.NonPublic)!;
    private static void Key(Control control, Keys key) => KeyDown.Invoke(control, [new KeyEventArgs(key, KeyEventType.KeyDown)]);
    private static void Require(bool condition, string message) { if (!condition) throw new InvalidOperationException(message); }

    public static string Run()
    {
        var results = new List<object>();
        void Check(string name, Action action)
        {
            try { action(); results.Add(new { name, passed = true }); }
            catch (Exception error) { results.Add(new { name, passed = false, error = error.ToString() }); }
        }
        Check("Editable title commit, cancel, assignment and disabled behavior", () =>
        {
            using var title = new FoundryEditableTitle("Untitled chat");
            var commits = 0;
            title.Committed += (_, _) => commits++;
            title.BeginEdit();
            var field = (FoundryFormField)title.Content!;
            // The native text editor is owned by the shared field; inspect it only in this host test.
            var editorField = typeof(FoundryEditableTitle).GetField("_editor", BindingFlags.NonPublic | BindingFlags.Instance)!;
            var editor = (TextBox)editorField.GetValue(title)!;
            editor.Text = "  My title  "; Key(editor, Keys.Enter);
            Require(title.Value == "My title" && commits == 1 && !title.IsEditing, "Commit must trim and fire once.");
            title.BeginEdit(); editor.Text = "discard"; Key(editor, Keys.Escape);
            Require(title.Value == "My title" && commits == 1, "Escape committed a change.");
            title.BeginEdit(); editor.Text = " "; Key(editor, Keys.Enter);
            Require(title.Value == "My title" && commits == 1, "Blank edit replaced the title.");
            title.BeginEdit(); title.Value = "Assigned";
            Require(!title.IsEditing && commits == 1, "Assignment must silently cancel editing.");
            title.BeginEdit(); title.Enabled = false;
            title.BeginEdit();
            Require(!title.IsEditing && commits == 1, "Disabled title accepted input.");
            title.Enabled = true; title.BeginEdit(); title.Dispose();
            Require(commits == 1, "Disposal committed an edit.");
        });
        Check("Preflight replacement and empty content", () =>
        {
            using var summary = new FoundryPreflightSummary();
            summary.SetSummary(["Floor plans"], new Dictionary<string, string> { ["Destination"] = "Root" });
            Require(summary.Content is StackLayout first && first.Items.Count == 2, "Missing preflight facts.");
            summary.SetSummary([], new Dictionary<string, string>());
            Require(summary.Content is StackLayout empty && empty.Items.Count == 0, "Stale preflight content.");
        });
        Check("Button keyboard and disabled parity", () =>
        {
            using var button = new FoundryDialogButton("Test", FoundryDialogButtonStyle.Secondary);
            var clicks = 0; button.Click += (_, _) => clicks++;
            Key(button, Keys.Space); Key(button, Keys.Enter);
            button.Enabled = false; Key(button, Keys.Space);
            Require(clicks == 2 && button.Height == 32, "32px action must activate twice and ignore disabled input.");
        });
        Check("Maps 34px variant keyboard parity", () =>
        {
            using var button = new FoundrySurfaceButton("Test");
            var clicks = 0; button.Click += (_, _) => clicks++;
            Key(button, Keys.Enter); button.Enabled = false; Key(button, Keys.Space);
            Require(clicks == 1 && button.Height == 34, "Maps action must retain its geometry and disabled behavior.");
            using var field = new FoundryInsetFormField(new TextBox());
            Require(field.Height == 34, "Maps field must remain 34px high.");
        });
        Check("Checkbox Space and disabled parity", () =>
        {
            using var checkbox = new FoundryCheckBox("Test");
            Key(checkbox, Keys.Space); Require(checkbox.Checked == true, "Space must toggle.");
            checkbox.Enabled = false; Key(checkbox, Keys.Space); Require(checkbox.Checked == true, "Disabled checkbox toggled.");
        });
        Check("Slider keyboard bounds and tooltip", () =>
        {
            using var slider = new FoundrySlider(0, 100, 50, toolTipFormatter: value => $"{value}% opacity");
            Key(slider, Keys.End); Require(slider.Value == 100 && slider.ToolTip == "100% opacity", "End must reach maximum.");
            Key(slider, Keys.Home); Require(slider.Value == 0, "Home must reach minimum.");
            slider.Enabled = false; Key(slider, Keys.End); Require(slider.Value == 0, "Disabled slider moved.");
        });
        Check("Accordion sections expand independently", () =>
        {
            using var first = new FoundryAccordionItem("One", new Label { Text = "One" }, true);
            using var second = new FoundryAccordionItem("Two", new Label { Text = "Two" }, true);
            first.IsExpanded = false;
            Require(!first.IsExpanded && second.IsExpanded, "Collapsing a section changed another section.");
        });
        Check("Gallery keyboard order and borrowed images", () =>
        {
            using var image = new Bitmap(24, 24, PixelFormat.Format32bppRgba);
            var gallery = new FoundryThumbnailGallery();
            gallery.SetItems([new("One", image), new("Two", null), new("Three", null)]);
            gallery.SetLayout(400, 120); gallery.SetSelectedName("One");
            string? selected = null; gallery.SelectionChanged += (_, e) => selected = e.Name;
            Key(gallery, Keys.End); Require(selected == "Three", "End must select last item.");
            Key(gallery, Keys.Home); Require(selected == "One", "Home must select first item.");
            gallery.Dispose(); Require(!image.IsDisposed, "Gallery disposed a borrowed image.");
        });
        Check("Canvas zoom anchor, pan batching and cancellation", () =>
        {
            using var canvas = new TestCanvas { Size = new Size(800, 600) };
            var anchor = new FoundryPoint(120, 180);
            var before = canvas.CanvasCamera.ScreenToWorld(anchor, new(800, 600));
            canvas.Zoom(2, anchor); canvas.Flush();
            Require(canvas.CanvasCamera.ScreenToWorld(anchor, new(800, 600)) == before, "Zoom moved the anchored world point.");
            var center = canvas.CanvasCamera.WorldCenter;
            canvas.Pan(10, 20); canvas.Pan(10, 20); canvas.Flush();
            Require(canvas.CanvasCamera.WorldCenter == center - new FoundryPoint(10, 20), "Pan events did not coalesce.");
            canvas.Zoom(2, anchor); canvas.Cancel(); canvas.Flush();
            Require(canvas.CanvasCamera.Zoom == 2, "Cancelled input was applied.");
        });
        return JsonSerializer.Serialize(results, new JsonSerializerOptions { WriteIndented = true });
    }

    public static Form ShowGallery()
    {
        var status = new Label { Text = "Ready. Use Tab, Enter, Space, arrows and scroll." };
        var questions = new FoundryQuestionSequence(new[] {
            new FoundryQuestion("How should the review proceed?", new[] { "Explain gains and losses (Recommended)", "Choose a primary goal" }) {
                Descriptions = new[] { "Summarize measured improvements and regressions without a weighted score.", "Choose a goal before testing and show the other trade-offs." }
            }, new FoundryQuestion("Include the optional comparison?", new[] { "Include", "Exclude" })
        });
        questions.Submitted += (_, _) => status.Text = string.Join("; ", questions.Answers);
        questions.Cancelled += (_, _) => status.Text = "Cancelled without submission";
        var button = new FoundryDialogButton("Action", FoundryDialogButtonStyle.Secondary);
        button.Click += (_, _) => status.Text = "Action activated";
        var canvas = new TestCanvas { Height = 180, BackgroundColor = FoundryTheme.CanvasBackground };
        canvas.Paint += (_, e) =>
        {
            var point = canvas.CanvasCamera.WorldToScreen(new FoundryPoint(0, 0), new(canvas.Width, canvas.Height));
            e.Graphics.DrawRectangle(FoundryTheme.PrimaryText, (float)point.X - 30, (float)point.Y - 20, 60, 40);
        };
        canvas.CameraChanged += (_, _) => status.Text = $"Canvas zoom {canvas.CanvasCamera.Zoom:0.00}";
        var content = new FoundryAccordion(
            new("Chat", new StackLayout { HorizontalContentAlignment = HorizontalAlignment.Stretch, Spacing = 12,
                Items = { questions, new FoundryChatMessage("Review this drawing. This message should wrap and align to the right.", true),
                    new FoundryMarkdownMessage("### Drawing review\n\n**Ready** — inspect `A01`.\n\n| Sheet | Status |\n|---|---|\n| A01 | Reuse existing view |\n| A02 | Needs a section |\n\n1. Check the scale.\n2. Review the framing."),
                    new FoundryChatComposer(new TextArea(), new FoundryDialogButton("Send", FoundryDialogButtonStyle.Secondary), new FoundryDialogButton("Stop", FoundryDialogButtonStyle.Secondary) { Enabled = false }) } }, true),
            new("Editable titles", new StackLayout { Spacing = 8, Items = {
                new FoundryEditableTitle("Untitled chat"),
                new FoundryEditableTitle("A long title that should be truncated without losing its full editable value"),
                new FoundryEditableTitle("Disabled title") { Enabled = false } } }),
            new("Actions and fields", new StackLayout { Spacing = 8, Items = {
                button, new FoundryDialogButton("Disabled", FoundryDialogButtonStyle.Secondary) { Enabled = false },
                new FoundryFormField(new TextBox { PlaceholderText = "32px field" }),
                new FoundryCheckBox("Space toggles this"), new FoundrySlider(0, 100, 50) } }, true),
            new("Maps variants", new StackLayout { Spacing = 8, Items = {
                new FoundrySurfaceButton("34px action"), new FoundryInsetFormField(new TextBox { PlaceholderText = "34px field" }) } }, true),
            new("Canvas: two-finger pan, pinch zoom", canvas, true));
        var form = new Form { Title = "Foundry UI parity gallery", ClientSize = new Size(540, 680),
            Content = new StackLayout { Padding = 12, Spacing = 8, HorizontalContentAlignment = HorizontalAlignment.Stretch,
                Items = { status, new StackLayoutItem(new FoundryScrollable(content), true) } } };
        form.Show();
        return form;
    }

    private sealed class TestCanvas : FoundryCanvas
    {
        public void Pan(double x, double y) => QueueCameraPan(x, y);
        public void Zoom(double factor, FoundryPoint anchor) => QueueCameraZoom(factor, anchor);
        public void Cancel() => StopQueuedCameraInput();
        public void Flush() => typeof(FoundryCanvas).GetMethod("FlushQueuedCameraInput", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(this, null);
    }
}
