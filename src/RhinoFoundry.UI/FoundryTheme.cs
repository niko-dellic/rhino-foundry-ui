using Eto.Drawing;
using Eto.Forms;
using Rhino.ApplicationSettings;

namespace RhinoFoundry.UI;

public static class FoundryTheme
{
    private const float HierarchyTableFontSize = 11;
    public const int TableRowHeight = 24;

    public const int Space1 = 4;
    public const int Space2 = 8;
    public const int Space3 = 12;
    public const int Space4 = 16;
    public const int Space6 = 24;

    public static Color PanelBackground => SystemColors.ControlBackground;

    public static Color ContentBackground => PanelBackground;

    public static Color PrimaryText => SystemColors.ControlText;

    public static Color SecondaryText => IsDarkMode
        ? Color.FromArgb(161, 161, 170, 255)
        : Color.FromArgb(82, 82, 91, 255);

    public static Color MutedText => IsDarkMode
        ? Color.FromArgb(113, 113, 122, 255)
        : Color.FromArgb(113, 113, 122, 255);

    public static bool IsDarkMode
    {
        get
        {
            var background = SystemColors.ControlBackground;
            return background.Rb * 299 + background.Gb * 587 + background.Bb * 114 < 128000;
        }
    }

    // The infinite board is part of the same workspace, so its base follows
    // Rhino's panel background. Grid and card tokens provide the spatial depth.
    public static Color CanvasBackground => PanelBackground;

    public static Color CanvasSurface => IsDarkMode
        ? Color.FromArgb(39, 39, 42, 255)
        : Color.FromArgb(255, 255, 255, 255);

    public static Color CanvasSubtleSurface => IsDarkMode
        ? Color.FromArgb(63, 63, 70, 255)
        : Color.FromArgb(244, 244, 245, 255);

    public static Color CanvasBorder => IsDarkMode
        ? Color.FromArgb(82, 82, 91, 255)
        : Color.FromArgb(212, 212, 216, 255);

    // Opaque chrome for canvas overlays that can sit above white sheet previews.
    public static Color CanvasOverlayBackground => IsDarkMode
        ? Color.FromArgb(36, 36, 36, 255)
        : Color.FromArgb(250, 250, 250, 255);

    public static Color CanvasFolderBackground => IsDarkMode
        ? Color.FromArgb(36, 36, 36, 255)
        : Color.FromArgb(250, 250, 250, 255);

    public static Color InputBackground => IsDarkMode
        ? Color.FromArgb(36, 36, 36, 255)
        : Color.FromArgb(255, 255, 255, 255);

    public static Color ToolbarActiveBackground => IsDarkMode
        ? Color.FromArgb(21, 21, 21, 255)
        : Color.FromArgb(228, 228, 231, 255);

    public static Color ToolbarButtonBackground => IsDarkMode
        ? Color.FromArgb(36, 36, 36, 255)
        : Color.FromArgb(250, 250, 250, 255);

    public static Color ToolbarGroupBackground => IsDarkMode
        ? Color.FromArgb(42, 42, 42, 255)
        : Color.FromArgb(244, 244, 245, 255);

    public static Color HierarchyFolderBackground => IsDarkMode
        ? Color.FromArgb(34, 34, 37, 255)
        : Color.FromArgb(250, 250, 250, 255);

    public static Color HierarchyAlternateRowBackground => IsDarkMode
        ? Color.FromArgb(34, 34, 37, 255)
        : Color.FromArgb(250, 250, 250, 255);

    public static Color HierarchyDocumentBackground => IsDarkMode
        ? Color.FromArgb(39, 39, 42, 255)
        : Color.FromArgb(244, 244, 245, 255);

    // Keep hierarchy reorganization feedback consistent with the native table
    // drag treatment instead of borrowing the blue canvas-selection accent.
    public static Color HierarchyDropBackground => SystemColors.Selection;

    public static Color HierarchyDropForeground => SystemColors.SelectionText;

    public static Color HierarchyDropStroke => IsDarkMode
        ? Color.FromArgb(212, 212, 216, 255)
        : Color.FromArgb(82, 82, 91, 255);

    public static Color HierarchyInlineEditorRowBackground => SystemColors.Selection;

    public static Color HierarchyInlineEditorRowForeground => SystemColors.SelectionText;

    public static Color HierarchyInlineEditorBackground => InputBackground;

    public static Color HierarchyInlineEditorForeground => PrimaryText;

    public static Color HierarchyInlineEditorSelectionBackground => SystemColors.Selection;

    public static Color HierarchyInlineEditorSelectionForeground => SystemColors.SelectionText;

    public static Color HierarchyInlineEditorStroke => WithAlpha(PrimaryText, 185);

    public const double DefaultCanvasGridOpacity = 0.80;

    public static Color CanvasGridColor => IsDarkMode
        ? Color.FromArgb(161, 161, 170, 255)
        : Color.FromArgb(82, 82, 91, 255);

    public static Color CanvasGrid => WithAlpha(
        CanvasGridColor,
        (int)Math.Round(DefaultCanvasGridOpacity * 255));

    public static Color SelectionAccent => RhinoColor(
        () => AppearanceSettings.SelectedObjectColor,
        Color.FromArgb(59, 130, 246, 255));

    public static Color DangerAccent => IsDarkMode
        ? Color.FromArgb(239, 68, 68, 255)
        : Color.FromArgb(220, 38, 38, 255);

    public static Color WarningAccent => IsDarkMode
        ? Color.FromArgb(251, 191, 36, 255)
        : Color.FromArgb(180, 83, 9, 255);

    public static Color WarningSurface => IsDarkMode
        ? Color.FromArgb(63, 42, 18, 255)
        : Color.FromArgb(255, 247, 237, 255);

    public static Color SelectionWindowStroke(bool crossing) => RhinoColor(
        () => crossing
            ? AppearanceSettings.SelectionWindowCrossingStrokeColor
            : AppearanceSettings.SelectionWindowStrokeColor,
        SelectionAccent);

    public static Color SelectionWindowFill(bool crossing) => RhinoColor(
        () => crossing
            ? AppearanceSettings.SelectionWindowCrossingFillColor
            : AppearanceSettings.SelectionWindowFillColor,
        WithAlpha(SelectionAccent, 42));

    public static Color WithAlpha(Color color, int alpha) =>
        Color.FromArgb(color.Rb, color.Gb, color.Bb, Math.Clamp(alpha, 0, 255));

    public static Font BrandFont => SystemFonts.Bold(9);

    public static Font EmptyTitleFont => SystemFonts.Bold(13);

    public static Font HierarchyTableFont => SystemFonts.Default(HierarchyTableFontSize);

    public static Font HierarchyTableBadgeFont => SystemFonts.Bold(HierarchyTableFontSize);

    public static Label MutedLabel(string text = "")
    {
        return new Label
        {
            Text = text,
            TextColor = MutedText,
            TextAlignment = TextAlignment.Left,
        };
    }

    public static Panel Surface(Control content, Padding? padding = null)
    {
        return new Panel
        {
            BackgroundColor = ContentBackground,
            Padding = padding ?? new Padding(0),
            Content = content,
        };
    }

    public static Control VerticalRule() => new Panel
    {
        BackgroundColor = CanvasBorder,
        Size = new Size(1, 20),
    };

    private static Color RhinoColor(Func<System.Drawing.Color> getColor, Color fallback)
    {
        try
        {
            var color = getColor();
            return Color.FromArgb(color.R, color.G, color.B, color.A);
        }
        catch
        {
            return fallback;
        }
    }
}
