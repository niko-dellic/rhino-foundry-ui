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
