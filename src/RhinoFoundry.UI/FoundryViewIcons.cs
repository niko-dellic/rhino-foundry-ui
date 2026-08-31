using Eto.Drawing;

namespace RhinoFoundry.UI;

/// <summary>
/// Resolution-independent icons for common plug-in workspace actions.
/// </summary>
public static class FoundryViewIcons
{
    private const int IconSize = 16;
    private const float Hairline = 0.8f;
    private const float Emphasis = 0.9f;
    private static readonly float[] IconScales = [1f, 2f, 3f];

    public static Icon Table() => NewIcon(graphics =>
    {
        var pen = new Pen(FoundryTheme.PrimaryText, Hairline);
        graphics.DrawRectangle(pen, 1.5f, 2, 13, 12);
        graphics.DrawLine(pen, 1.5f, 6, 14.5f, 6);
        graphics.DrawLine(pen, 1.5f, 10, 14.5f, 10);
        graphics.DrawLine(pen, 6, 2, 6, 14);
    });

    public static Icon Thumbnails() => NewIcon(graphics =>
    {
        var pen = new Pen(FoundryTheme.PrimaryText, Hairline);
        graphics.DrawRectangle(pen, 1.5f, 2, 5.5f, 5);
        graphics.DrawRectangle(pen, 9, 2, 5.5f, 5);
        graphics.DrawRectangle(pen, 1.5f, 9, 5.5f, 5);
        graphics.DrawRectangle(pen, 9, 9, 5.5f, 5);
    });

    public static Icon Canvas() => NewIcon(graphics =>
    {
        var color = FoundryTheme.PrimaryText;
        var grid = FoundryTheme.WithAlpha(color, 110);
        foreach (var x in new[] { 7f, 11f })
        foreach (var y in new[] { 4f, 8f })
            graphics.FillEllipse(grid, x, y, 1.5f, 1.5f);
        var pen = new Pen(color, Emphasis);
        graphics.DrawLine(pen, 3, 12.5f, 14, 12.5f);
        graphics.DrawLine(pen, 3.5f, 13, 3.5f, 2);
        graphics.DrawLine(pen, 14, 12.5f, 11.5f, 10.5f);
        graphics.DrawLine(pen, 3.5f, 2, 1.5f, 4.5f);
    });

    public static Icon Search() => NewIcon(graphics =>
    {
        var pen = new Pen(FoundryTheme.PrimaryText, Emphasis);
        graphics.DrawEllipse(pen, 2, 2, 8.5f, 8.5f);
        graphics.DrawLine(pen, 9.25f, 9.25f, 14, 14);
    });

    public static Icon Refresh() => NewIcon(graphics =>
    {
        var pen = new Pen(FoundryTheme.PrimaryText, Emphasis);
        graphics.DrawArc(pen, new RectangleF(2, 2, 12, 12), 35, 285);
        graphics.DrawLine(pen, 12.25f, 1.75f, 14, 5);
        graphics.DrawLine(pen, 14, 5, 10.5f, 5.25f);
    });

    public static Icon Cube() => NewIcon(graphics =>
    {
        var pen = new Pen(FoundryTheme.PrimaryText, Emphasis);
        graphics.DrawLine(pen, 8, 1.5f, 14, 5);
        graphics.DrawLine(pen, 14, 5, 8, 8.5f);
        graphics.DrawLine(pen, 8, 8.5f, 2, 5);
        graphics.DrawLine(pen, 2, 5, 8, 1.5f);
        graphics.DrawLine(pen, 2, 5, 2, 11.5f);
        graphics.DrawLine(pen, 2, 11.5f, 8, 15);
        graphics.DrawLine(pen, 8, 15, 14, 11.5f);
        graphics.DrawLine(pen, 14, 11.5f, 14, 5);
        graphics.DrawLine(pen, 8, 8.5f, 8, 15);
    });

    public static Icon FitAll() => NewIcon(graphics => DrawCornerFrame(
        graphics,
        new Pen(FoundryTheme.PrimaryText, Hairline)));

    public static Icon FocusSelection() => NewIcon(graphics =>
    {
        var color = FoundryTheme.PrimaryText;
        var pen = new Pen(color, Hairline);
        graphics.DrawEllipse(pen, 3, 3, 10, 10);
        graphics.DrawEllipse(pen, 6, 6, 4, 4);
        graphics.DrawLine(pen, 8, 1, 8, 4);
        graphics.DrawLine(pen, 8, 12, 8, 15);
        graphics.DrawLine(pen, 1, 8, 4, 8);
        graphics.DrawLine(pen, 12, 8, 15, 8);
    });

    public static Icon ZoomOut() => ZoomGlyph(includePlus: false);

    public static Icon ZoomIn() => ZoomGlyph(includePlus: true);

    public static Icon Fullscreen() => NewIcon(graphics => DrawCornerFrame(
        graphics,
        new Pen(FoundryTheme.PrimaryText, Hairline)));

    public static Icon ExitFullscreen() => NewIcon(graphics =>
    {
        var pen = new Pen(FoundryTheme.PrimaryText, Hairline);
        graphics.DrawLine(pen, 6, 1.5f, 6, 6);
        graphics.DrawLine(pen, 6, 6, 1.5f, 6);
        graphics.DrawLine(pen, 10, 1.5f, 10, 6);
        graphics.DrawLine(pen, 10, 6, 14.5f, 6);
        graphics.DrawLine(pen, 10, 14.5f, 10, 10);
        graphics.DrawLine(pen, 10, 10, 14.5f, 10);
        graphics.DrawLine(pen, 6, 14.5f, 6, 10);
        graphics.DrawLine(pen, 6, 10, 1.5f, 10);
    });

    private static Icon ZoomGlyph(bool includePlus) => NewIcon(graphics =>
    {
        var pen = new Pen(FoundryTheme.PrimaryText, Hairline);
        graphics.DrawEllipse(pen, 2, 2, 9, 9);
        graphics.DrawLine(pen, 9.5f, 9.5f, 14, 14);
        graphics.DrawLine(pen, 4.25f, 6.5f, 8.75f, 6.5f);
        if (includePlus)
            graphics.DrawLine(pen, 6.5f, 4.25f, 6.5f, 8.75f);
    });

    private static void DrawCornerFrame(Graphics graphics, Pen pen)
    {
        graphics.DrawLine(pen, 2, 6, 2, 2);
        graphics.DrawLine(pen, 2, 2, 6, 2);
        graphics.DrawLine(pen, 10, 2, 14, 2);
        graphics.DrawLine(pen, 14, 2, 14, 6);
        graphics.DrawLine(pen, 14, 10, 14, 14);
        graphics.DrawLine(pen, 14, 14, 10, 14);
        graphics.DrawLine(pen, 6, 14, 2, 14);
        graphics.DrawLine(pen, 2, 14, 2, 10);
    }

    private static Icon NewIcon(Action<Graphics> draw)
    {
        var frames = new IconFrame[IconScales.Length];
        for (var index = 0; index < IconScales.Length; index++)
        {
            var scale = IconScales[index];
            var bitmap = new Bitmap(
                (int)(IconSize * scale),
                (int)(IconSize * scale),
                PixelFormat.Format32bppRgba);
            using var graphics = new Graphics(bitmap) { AntiAlias = true };
            graphics.ScaleTransform(scale);
            draw(graphics);
            frames[index] = new IconFrame(scale, bitmap);
        }

        return new Icon(frames);
    }
}
