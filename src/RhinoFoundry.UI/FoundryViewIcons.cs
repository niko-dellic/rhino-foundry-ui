using Eto.Drawing;

namespace RhinoFoundry.UI;

public static class FoundryViewIcons
{
    /// <summary>Send arrow. Inverse uses the composer action foreground; caller owns the icon.</summary>
    public static Icon Send(bool inverse = false) => NewIcon(graphics =>
    {
        using var pen = new Pen(inverse ? FoundryTheme.PanelBackground : FoundryTheme.PrimaryText, 1.6f);
        graphics.DrawLine(pen, 8, 14, 8, 2);
        graphics.DrawLine(pen, 8, 2, 3, 7);
        graphics.DrawLine(pen, 8, 2, 13, 7);
    });

    /// <summary>Stop square. Inverse uses the composer action foreground; caller owns the icon.</summary>
    public static Icon Stop(bool inverse = false) => NewIcon(graphics =>
        graphics.FillRectangle(inverse ? FoundryTheme.PanelBackground : FoundryTheme.PrimaryText, 4, 4, 8, 8));

    /// <summary>Generic camera icon; caller owns the returned image.</summary>
    public static Icon Camera() => NewIcon(graphics =>
    {
        using var pen = new Pen(FoundryTheme.PrimaryText, 1f);
        graphics.DrawLines(pen, new PointF(1.5f, 5), new PointF(4.5f, 5),
            new PointF(6, 3), new PointF(10, 3), new PointF(11.5f, 5),
            new PointF(14.5f, 5), new PointF(14.5f, 13),
            new PointF(1.5f, 13), new PointF(1.5f, 5));
        graphics.DrawEllipse(pen, 5, 6, 6, 6);
    });

    /// <summary>Resolution-independent conversation bubble.</summary>
    public static Icon Conversation() => NewIcon(graphics =>
    {
        using var pen = new Pen(FoundryTheme.PrimaryText, 1);
        using var path = new GraphicsPath();
        path.MoveTo(3, 2); path.LineTo(13, 2); path.LineTo(14, 3);
        path.LineTo(14, 10); path.LineTo(13, 11); path.LineTo(7, 11);
        path.LineTo(3, 14); path.LineTo(3, 11); path.LineTo(2, 10);
        path.LineTo(2, 3); path.CloseFigure();
        graphics.DrawPath(pen, path);
    });
    private const int IconSize = 16;
    private const int BrandMarkSize = 20;
    private const float Hairline = 0.8f;
    private const float Emphasis = 0.9f;
    private static readonly float[] IconScales = [1f, 2f, 3f];

    public static Icon ListView() => NewIcon(graphics =>
    {
        var color = FoundryTheme.PrimaryText;
        var pen = new Pen(color, Hairline);
        graphics.FillEllipse(color, 1.25f, 3.25f, 1.5f, 1.5f);
        graphics.FillEllipse(color, 1.25f, 7.25f, 1.5f, 1.5f);
        graphics.FillEllipse(color, 1.25f, 11.25f, 1.5f, 1.5f);
        graphics.DrawLine(pen, 4, 4, 14, 4);
        graphics.DrawLine(pen, 4, 8, 14, 8);
        graphics.DrawLine(pen, 4, 12, 14, 12);
    });

    public static Icon Search() => NewIcon(graphics =>
    {
        var color = FoundryTheme.PrimaryText;
        var pen = new Pen(color, Emphasis);
        graphics.DrawEllipse(pen, 2, 2, 8.5f, 8.5f);
        graphics.DrawLine(pen, 9.25f, 9.25f, 14, 14);
    });

    public static Icon Help() => NewIcon(graphics =>
    {
        var color = FoundryTheme.PrimaryText;
        var pen = new Pen(color, Emphasis);
        graphics.DrawArc(pen, new RectangleF(4.5f, 2, 7, 7), 205, 220);
        graphics.DrawLine(pen, 8, 8, 8, 10.5f);
        graphics.FillEllipse(color, 7.25f, 12, 1.5f, 1.5f);
    });

    public static Icon ThumbnailStack() => NewIcon(graphics =>
    {
        var color = FoundryTheme.PrimaryText;
        var muted = FoundryTheme.WithAlpha(color, 145);
        graphics.DrawRectangle(new Pen(muted, Hairline), 2.5f, 2.5f, 10, 2.5f);
        graphics.DrawRectangle(new Pen(muted, Hairline), 3.5f, 6.5f, 10, 2.5f);
        graphics.DrawRectangle(new Pen(color, Emphasis), 4.5f, 10.5f, 9, 2.5f);
    });

    public static Icon CartesianPlane() => NewIcon(graphics =>
    {
        var color = FoundryTheme.PrimaryText;
        var grid = FoundryTheme.WithAlpha(color, 110);
        graphics.FillEllipse(grid, 7, 4, 1.5f, 1.5f);
        graphics.FillEllipse(grid, 11, 4, 1.5f, 1.5f);
        graphics.FillEllipse(grid, 7, 8, 1.5f, 1.5f);
        graphics.FillEllipse(grid, 11, 8, 1.5f, 1.5f);
        var axisPen = new Pen(color, Emphasis);
        graphics.DrawLine(axisPen, 3, 12.5f, 14, 12.5f);
        graphics.DrawLine(axisPen, 3.5f, 13, 3.5f, 2);
        graphics.DrawLine(axisPen, 14, 12.5f, 11.5f, 10.5f);
        graphics.DrawLine(axisPen, 14, 12.5f, 11.5f, 14.5f);
        graphics.DrawLine(axisPen, 3.5f, 2, 1.5f, 4.5f);
        graphics.DrawLine(axisPen, 3.5f, 2, 5.5f, 4.5f);
        graphics.FillEllipse(color, 2.75f, 11.75f, 1.5f, 1.5f);
    });

    public static Icon NewFolder() => NewIcon(graphics =>
    {
        var color = FoundryTheme.PrimaryText;
        var pen = new Pen(color, Hairline);
        graphics.DrawLine(pen, 1.5f, 5, 1.5f, 13.5f);
        graphics.DrawLine(pen, 1.5f, 13.5f, 11.5f, 13.5f);
        graphics.DrawLine(pen, 11.5f, 13.5f, 11.5f, 6);
        graphics.DrawLine(pen, 1.5f, 5, 5, 5);
        graphics.DrawLine(pen, 5, 5, 6.5f, 6.5f);
        graphics.DrawLine(pen, 6.5f, 6.5f, 11.5f, 6.5f);
        DrawPlus(graphics, color, 12.5f, 3.5f);
    });

    public static Icon Add() => NewIcon(graphics =>
        DrawPlus(graphics, FoundryTheme.PrimaryText, 8, 8));

    public static Icon Folder() => NewIcon(graphics =>
    {
        var pen = new Pen(FoundryTheme.PrimaryText, Emphasis);
        graphics.DrawLine(pen, 1.5f, 4.5f, 6, 4.5f);
        graphics.DrawLine(pen, 6, 4.5f, 7.5f, 6);
        graphics.DrawLine(pen, 7.5f, 6, 14.5f, 6);
        graphics.DrawLine(pen, 14.5f, 6, 14.5f, 13.5f);
        graphics.DrawLine(pen, 14.5f, 13.5f, 1.5f, 13.5f);
        graphics.DrawLine(pen, 1.5f, 13.5f, 1.5f, 4.5f);
    });

    public static Icon Layout() => NewIcon(graphics =>
    {
        var pen = new Pen(FoundryTheme.PrimaryText, Emphasis);
        graphics.DrawRectangle(pen, 2.5f, 1.5f, 11, 13);
        graphics.DrawRectangle(new Pen(FoundryTheme.PrimaryText, Hairline), 4.5f, 4.5f, 7, 7);
    });

    public static Icon AppearanceState() => NewIcon(graphics =>
    {
        var color = FoundryTheme.PrimaryText;
        var pen = new Pen(color, Hairline);
        graphics.DrawLine(pen, 2, 4, 8, 1.75f);
        graphics.DrawLine(pen, 8, 1.75f, 14, 4);
        graphics.DrawLine(pen, 14, 4, 8, 6.25f);
        graphics.DrawLine(pen, 8, 6.25f, 2, 4);
        graphics.DrawLine(pen, 2, 8, 8, 10.25f);
        graphics.DrawLine(pen, 8, 10.25f, 14, 8);
        graphics.DrawEllipse(new Pen(color, Emphasis), 9.75f, 10.25f, 4, 4);
    });

    public static Icon AppearanceCards() => NewIcon(graphics =>
    {
        var color = FoundryTheme.PrimaryText;
        var pen = new Pen(color, Emphasis);
        graphics.DrawRectangle(pen, 2, 3, 12, 10);
        graphics.DrawLine(new Pen(color, Hairline), 5, 6, 11, 6);
        graphics.DrawLine(new Pen(color, Hairline), 5, 9, 9, 9);
    });

    public static Icon AppearanceConnections() => NewIcon(graphics =>
    {
        var color = FoundryTheme.PrimaryText;
        var pen = new Pen(color, Emphasis);
        graphics.DrawRectangle(pen, 1.5f, 2.5f, 5, 4);
        graphics.DrawRectangle(pen, 9.5f, 9.5f, 5, 4);
        graphics.DrawLine(pen, 6.5f, 4.5f, 11.5f, 4.5f);
        graphics.DrawLine(pen, 11.5f, 4.5f, 11.5f, 9.5f);
    });

    public static Icon AppearanceBadges() => NewIcon(graphics =>
    {
        var color = FoundryTheme.PrimaryText;
        var pen = new Pen(color, Emphasis);
        graphics.DrawRectangle(pen, 1.5f, 3, 13, 10);
        graphics.DrawRectangle(new Pen(color, Hairline), 8, 5.5f, 5, 3.5f);
        graphics.FillEllipse(color, 3.5f, 6, 2, 2);
    });

    public static Icon SceneCursor() => NewIcon(graphics =>
    {
        var color = FoundryTheme.PrimaryText;
        var pen = new Pen(color, Emphasis);
        graphics.DrawLines(pen,
        [
            new PointF(2.25f, 5), new PointF(2.25f, 2.25f), new PointF(5, 2.25f),
        ]);
        graphics.DrawLines(pen,
        [
            new PointF(11, 2.25f), new PointF(13.75f, 2.25f), new PointF(13.75f, 5),
        ]);
        graphics.DrawLines(pen,
        [
            new PointF(13.75f, 11), new PointF(13.75f, 13.75f), new PointF(11, 13.75f),
        ]);
        graphics.DrawLines(pen,
        [
            new PointF(5, 13.75f), new PointF(2.25f, 13.75f), new PointF(2.25f, 11),
        ]);
        graphics.DrawPolygon(pen,
        [
            new PointF(6.1f, 5.15f),
            new PointF(6.1f, 11.3f),
            new PointF(7.7f, 9.75f),
            new PointF(9.05f, 12.8f),
            new PointF(10.35f, 12.2f),
            new PointF(9.05f, 9.25f),
            new PointF(11.25f, 9.25f),
        ]);
    });

    public static Icon VisibilityOn() => VisibilityCircle(fill: true, half: false);

    public static Icon VisibilityOff() => VisibilityCircle(fill: false, half: false);

    public static Icon NewLayout() => NewIcon(graphics =>
    {
        var color = FoundryTheme.PrimaryText;
        graphics.DrawRectangle(new Pen(color, Emphasis), 1.5f, 3.5f, 9, 10.5f);
        graphics.DrawRectangle(new Pen(color, Hairline), 3.5f, 6, 5, 5.5f);
        DrawPlus(graphics, color, 12.5f, 3.5f);
    });

    private static Icon VisibilityCircle(bool fill, bool half) => NewIcon(graphics =>
    {
        var color = FoundryTheme.PrimaryText;
        if (fill) graphics.FillEllipse(color, 3, 3, 10, 10);
        else graphics.DrawEllipse(new Pen(color, Emphasis), 3, 3, 10, 10);
        if (!half) return;
        graphics.FillPolygon(color,
        [
            new PointF(8, 3),
            new PointF(4.5f, 4.5f),
            new PointF(3, 8),
            new PointF(4.5f, 11.5f),
            new PointF(8, 13),
            new PointF(8, 3),
        ]);
    });

    public static Icon Properties() => NewIcon(graphics =>
    {
        var color = FoundryTheme.PrimaryText;
        var pen = new Pen(color, Hairline);
        graphics.DrawLine(pen, 2, 4, 14, 4);
        graphics.DrawLine(pen, 2, 8, 14, 8);
        graphics.DrawLine(pen, 2, 12, 14, 12);
        graphics.DrawEllipse(pen, 4.5f, 2, 4, 4);
        graphics.DrawEllipse(pen, 9.5f, 6, 4, 4);
        graphics.DrawEllipse(pen, 6.5f, 10, 4, 4);
    });

    public static Icon ProjectInformation() => NewIcon(graphics =>
    {
        var color = FoundryTheme.PrimaryText;
        var muted = FoundryTheme.WithAlpha(color, 155);
        var pen = new Pen(color, Emphasis);
        graphics.DrawRectangle(pen, 2, 1.5f, 12, 13);
        graphics.DrawEllipse(new Pen(muted, Hairline), 4, 4, 3, 3);
        graphics.DrawLine(new Pen(muted, Hairline), 3.75f, 9, 8, 9);
        graphics.DrawLine(new Pen(muted, Hairline), 3.75f, 11.5f, 8, 11.5f);
        graphics.DrawLine(pen, 9.5f, 5, 12.25f, 5);
        graphics.DrawLine(pen, 9.5f, 7.5f, 12.25f, 7.5f);
        graphics.DrawLine(pen, 9.5f, 10, 12.25f, 10);
    });

    public static Icon Delete() => NewIcon(graphics =>
    {
        var color = FoundryTheme.PrimaryText;
        var pen = new Pen(color, Hairline);
        graphics.DrawLine(pen, 3, 4.5f, 13, 4.5f);
        graphics.DrawLine(pen, 6, 2.5f, 10, 2.5f);
        graphics.DrawLine(pen, 6, 2.5f, 5.5f, 4.5f);
        graphics.DrawLine(pen, 10, 2.5f, 10.5f, 4.5f);
        graphics.DrawRectangle(pen, 4.5f, 5.5f, 7, 8);
        graphics.DrawLine(pen, 7, 7, 7, 12);
        graphics.DrawLine(pen, 9, 7, 9, 12);
    });

    public static Icon ClearSelection() => NewIcon(graphics =>
    {
        var color = FoundryTheme.PrimaryText;
        var pen = new Pen(color, Hairline);
        graphics.DrawLine(pen, 2, 5, 2, 2);
        graphics.DrawLine(pen, 2, 2, 5, 2);
        graphics.DrawLine(pen, 11, 2, 14, 2);
        graphics.DrawLine(pen, 14, 2, 14, 5);
        graphics.DrawLine(pen, 2, 11, 2, 14);
        graphics.DrawLine(pen, 2, 14, 5, 14);
        graphics.DrawLine(pen, 11, 14, 14, 14);
        graphics.DrawLine(pen, 14, 14, 14, 11);
        graphics.DrawLine(pen, 6, 6, 10, 10);
        graphics.DrawLine(pen, 10, 6, 6, 10);
    });

    public static Icon ImportPackage() => TransferPackage(arrowPointsDown: true);

    public static Icon ExportPackage() => TransferPackage(arrowPointsDown: false);

    public static Icon Print() => NewIcon(graphics =>
    {
        var color = FoundryTheme.PrimaryText;
        var pen = new Pen(color, Emphasis);
        graphics.DrawRectangle(pen, 3.5f, 1.5f, 9, 5.5f);
        graphics.DrawLine(pen, 1.5f, 6, 14.5f, 6);
        graphics.DrawLine(pen, 1.5f, 6, 1.5f, 12.5f);
        graphics.DrawLine(pen, 14.5f, 6, 14.5f, 12.5f);
        graphics.DrawLine(pen, 1.5f, 12.5f, 3.5f, 12.5f);
        graphics.DrawLine(pen, 12.5f, 12.5f, 14.5f, 12.5f);
        graphics.FillEllipse(color, 11.5f, 8, 1.25f, 1.25f);
        graphics.DrawRectangle(pen, 3.5f, 10, 9, 4.5f);
        graphics.DrawLine(new Pen(color, Hairline), 5, 12, 11, 12);
    });

    public static Icon FitAll() => NewIcon(graphics =>
    {
        var pen = new Pen(FoundryTheme.PrimaryText, Hairline);
        DrawCornerFrame(graphics, pen);
    });

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

    public static Icon Tidy() => NewIcon(graphics =>
    {
        var pen = new Pen(FoundryTheme.PrimaryText, Hairline);
        graphics.DrawRectangle(pen, 2, 2, 5, 5);
        graphics.DrawRectangle(pen, 9, 2, 5, 5);
        graphics.DrawRectangle(pen, 2, 9, 5, 5);
        graphics.DrawRectangle(pen, 9, 9, 5, 5);
    });

    public static Icon NestedPacking() => NewIcon(graphics =>
    {
        var pen = new Pen(FoundryTheme.PrimaryText, Hairline);
        graphics.DrawRectangle(pen, 1.5f, 2, 13, 12.5f);
        graphics.DrawLine(pen, 1.5f, 5, 14.5f, 5);
        graphics.DrawRectangle(pen, 4, 7, 8, 5);
        graphics.DrawLine(pen, 4, 8.75f, 12, 8.75f);
    });

    public static Icon CompactPacking() => NewIcon(graphics =>
    {
        var pen = new Pen(FoundryTheme.PrimaryText, Hairline);
        graphics.DrawRectangle(pen, 1.5f, 2, 6, 5);
        graphics.DrawRectangle(pen, 8.5f, 2, 6, 5);
        graphics.DrawRectangle(pen, 1.5f, 8, 6, 5);
        graphics.DrawRectangle(pen, 8.5f, 8, 6, 5);
    });

    public static Icon GridAppearance() => NewIcon(graphics =>
    {
        var color = FoundryTheme.PrimaryText;
        var muted = FoundryTheme.WithAlpha(color, 135);
        foreach (var x in new[] { 2.5f, 7f, 11.5f })
        foreach (var y in new[] { 2.5f, 7f, 11.5f })
            graphics.FillEllipse(muted, x, y, 1.5f, 1.5f);

        graphics.DrawEllipse(new Pen(color, Emphasis), 9.25f, 9.25f, 5.25f, 5.25f);
        graphics.FillEllipse(color, 11.1f, 11.1f, 1.6f, 1.6f);
    });

    public static Icon ZoomOut() => ZoomGlyph(includePlus: false);

    public static Icon ZoomIn() => ZoomGlyph(includePlus: true);

    public static Icon Navigator() => NewIcon(graphics =>
    {
        var color = FoundryTheme.PrimaryText;
        var pen = new Pen(color, Hairline);
        graphics.DrawLine(pen, 4, 4, 13.5f, 4);
        graphics.DrawLine(pen, 4, 8, 13.5f, 8);
        graphics.DrawLine(pen, 4, 12, 13.5f, 12);
        graphics.FillEllipse(color, 1.25f, 3.25f, 1.5f, 1.5f);
        graphics.FillEllipse(color, 1.25f, 7.25f, 1.5f, 1.5f);
        graphics.FillEllipse(color, 1.25f, 11.25f, 1.5f, 1.5f);
    });

    public static Icon NamedViews() => NewIcon(graphics =>
    {
        var color = FoundryTheme.PrimaryText;
        var pen = new Pen(color, Hairline);
        graphics.DrawRectangle(pen, 1.5f, 2.5f, 13, 11);
        graphics.FillEllipse(color, 10.75f, 4.75f, 1.5f, 1.5f);
        graphics.DrawLine(pen, 3, 11.5f, 6.25f, 7.5f);
        graphics.DrawLine(pen, 6.25f, 7.5f, 8.5f, 10);
        graphics.DrawLine(pen, 8.5f, 10, 10, 8.5f);
        graphics.DrawLine(pen, 10, 8.5f, 13, 11.5f);
    });

    public static Icon OpenSelection() => NewIcon(graphics =>
    {
        var pen = new Pen(FoundryTheme.PrimaryText, Hairline);
        graphics.DrawLine(pen, 3, 6, 3, 13);
        graphics.DrawLine(pen, 3, 13, 10, 13);
        graphics.DrawLine(pen, 7, 3, 13, 3);
        graphics.DrawLine(pen, 13, 3, 13, 9);
        graphics.DrawLine(pen, 7, 9, 13, 3);
    });

    public static Icon Fullscreen() => NewIcon(graphics =>
    {
        var pen = new Pen(FoundryTheme.PrimaryText, Hairline);
        DrawCornerFrame(graphics, pen);
    });

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

    public static Icon Pencil() => NewIcon(graphics =>
    {
        using var pen = new Pen(FoundryTheme.SecondaryText, Hairline);
        graphics.DrawLine(pen, 3, 10, 10, 3);
        graphics.DrawLine(pen, 10, 3, 13, 6);
        graphics.DrawLine(pen, 13, 6, 6, 13);
        graphics.DrawLine(pen, 6, 13, 2, 14);
        graphics.DrawLine(pen, 2, 14, 3, 10);
        graphics.DrawLine(pen, 9, 4, 12, 7);
    });

    public static Icon Close() => NewIcon(graphics =>
    {
        var pen = new Pen(FoundryTheme.PrimaryText, Emphasis);
        graphics.DrawLine(pen, 4, 4, 12, 12);
        graphics.DrawLine(pen, 12, 4, 4, 12);
    });

    public static Icon More() => NewIcon(graphics =>
    {
        var color = FoundryTheme.PrimaryText;
        graphics.FillEllipse(color, 2.25f, 7.25f, 1.5f, 1.5f);
        graphics.FillEllipse(color, 7.25f, 7.25f, 1.5f, 1.5f);
        graphics.FillEllipse(color, 12.25f, 7.25f, 1.5f, 1.5f);
    });

    public static Icon ChevronDown() => NewIcon(graphics =>
    {
        var pen = new Pen(FoundryTheme.PrimaryText, Emphasis);
        graphics.DrawLine(pen, 4, 6, 8, 10);
        graphics.DrawLine(pen, 8, 10, 12, 6);
    });

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


    private static Icon ZoomGlyph(bool includePlus) => NewIcon(graphics =>
    {
        var pen = new Pen(FoundryTheme.PrimaryText, Hairline);
        graphics.DrawEllipse(pen, 2, 2, 9, 9);
        graphics.DrawLine(pen, 9.5f, 9.5f, 14, 14);
        graphics.DrawLine(pen, 4.25f, 6.5f, 8.75f, 6.5f);
        if (includePlus)
            graphics.DrawLine(pen, 6.5f, 4.25f, 6.5f, 8.75f);
    });

    private static Icon TransferPackage(bool arrowPointsDown) => NewIcon(graphics =>
    {
        var pen = new Pen(FoundryTheme.PrimaryText, 1.15f);
        graphics.DrawRectangle(pen, 2, 8.5f, 12, 5.5f);
        graphics.DrawLine(pen, 5, 11, 11, 11);
        if (arrowPointsDown)
        {
            graphics.DrawLine(pen, 8, 1.5f, 8, 8);
            graphics.DrawLine(pen, 5.5f, 5.5f, 8, 8);
            graphics.DrawLine(pen, 10.5f, 5.5f, 8, 8);
        }
        else
        {
            graphics.DrawLine(pen, 8, 8, 8, 1.5f);
            graphics.DrawLine(pen, 5.5f, 4, 8, 1.5f);
            graphics.DrawLine(pen, 10.5f, 4, 8, 1.5f);
        }
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

    private static void DrawPlus(Graphics graphics, Color color, float x, float y)
    {
        var pen = new Pen(color, Emphasis);
        graphics.DrawLine(pen, x - 2.5f, y, x + 2.5f, y);
        graphics.DrawLine(pen, x, y - 2.5f, x, y + 2.5f);
    }

    private static Icon NewIcon(Action<Graphics> draw, int size = IconSize)
    {
        var frames = new IconFrame[IconScales.Length];
        for (var index = 0; index < IconScales.Length; index++)
        {
            var scale = IconScales[index];
            var bitmap = new Bitmap(
                (int)(size * scale),
                (int)(size * scale),
                PixelFormat.Format32bppRgba);
            using var graphics = new Graphics(bitmap) { AntiAlias = true };
            graphics.ScaleTransform(scale);
            draw(graphics);
            frames[index] = new IconFrame(scale, bitmap);
        }

        return new Icon(frames);
    }
}
