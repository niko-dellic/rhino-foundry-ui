using Eto.Drawing;

namespace RhinoFoundry.UI;

/// <summary>Product-neutral shortcut glyphs. Coordinates are logical pixels with centered boxes/text.</summary>
/// <remarks>The caller owns scaling, opacity, rendering and viewport lifecycle. This never takes focus.</remarks>
public static class FoundryShortcutDrawing
{
    public delegate void Box(float x, float y, float width, float height, float radius, Color stroke, Color fill);
    public delegate void Line(float x1, float y1, float x2, float y2, Color color);
    public delegate void Text(string text, float x, float y, int size, Color color);

    public static void Keycap(string label, float x, float y, float width, Box box, Text text)
    {
        box(x, y, width, 21, 4, FoundryTheme.CanvasBorder, FoundryTheme.InputBackground);
        text(label, x, y, 10, FoundryTheme.PrimaryText);
    }

    public static void Mouse(float x, float y, Box box, Line line)
    {
        var edge = FoundryTheme.CanvasBorder;
        box(x, y, 27, 44, 12, edge, FoundryTheme.InputBackground);
        line(x - 13.5f, y - 2, x + 13.5f, y - 2, edge);
        line(x, y - 22, x, y - 12, edge);
        box(x, y - 9, 4, 9, 2, edge, FoundryTheme.SecondaryText);
    }
}
