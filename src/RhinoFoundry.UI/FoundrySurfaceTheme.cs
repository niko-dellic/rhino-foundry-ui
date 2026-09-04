using Eto.Drawing;
using Eto.Forms;
using Rhino.ApplicationSettings;

namespace RhinoFoundry.UI;

public static class FoundrySurfaceTheme
{
    public const int Space1 = 4;
    public const int Space2 = 8;
    public const int Space3 = 12;
    public const int Space4 = 16;

    public static bool IsDarkMode
    {
        get
        {
            var background = SystemColors.ControlBackground;
            return background.Rb * 299 + background.Gb * 587 + background.Bb * 114 < 128000;
        }
    }

    public static Color PanelBackground => SystemColors.ControlBackground;
    public static Color Text => SystemColors.ControlText;
    public static Color MutedText => IsDarkMode
        ? Color.FromArgb(161, 161, 170, 255)
        : Color.FromArgb(82, 82, 91, 255);
    public static Color Surface => IsDarkMode
        ? Color.FromArgb(30, 30, 32, 255)
        : Color.FromArgb(250, 250, 250, 255);
    public static Color HoverSurface => IsDarkMode
        ? Color.FromArgb(63, 63, 70, 255)
        : Color.FromArgb(244, 244, 245, 255);
    public static Color Border => IsDarkMode
        ? Color.FromArgb(82, 82, 91, 255)
        : Color.FromArgb(212, 212, 216, 255);
    public static Color Active => IsDarkMode
        ? Color.FromArgb(21, 21, 21, 255)
        : Color.FromArgb(228, 228, 231, 255);
    public static Color Focus => IsDarkMode
        ? Color.FromArgb(212, 212, 216, 255)
        : Color.FromArgb(63, 63, 70, 255);
    public static Color WarningSurface => IsDarkMode
        ? Color.FromArgb(42, 34, 19, 255)
        : Color.FromArgb(255, 251, 235, 255);
    public static Color WarningBorder => IsDarkMode
        ? Color.FromArgb(146, 101, 16, 255)
        : Color.FromArgb(245, 158, 11, 255);
    public static Color WarningText => IsDarkMode
        ? Color.FromArgb(253, 230, 138, 255)
        : Color.FromArgb(146, 64, 14, 255);

    public static Color WithAlpha(Color color, int alpha) =>
        Color.FromArgb(color.Rb, color.Gb, color.Bb, Math.Clamp(alpha, 0, 255));
}
