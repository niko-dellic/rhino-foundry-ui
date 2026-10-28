namespace RhinoFoundry.UI.Primitives;

/// <summary>Pure animation timing; the consumer owns timers, drawing and dismissal.</summary>
public static class FoundryNoticeTiming
{
    public static double SlideOffset(double elapsedSeconds, double duration = 0.3)
    {
        if (!double.IsFinite(duration) || duration <= 0) throw new ArgumentOutOfRangeException(nameof(duration));
        return Math.Pow(1 - Math.Clamp(elapsedSeconds / duration, 0, 1), 3);
    }
    public static double Opacity(double elapsedSeconds, double hold = 3, double fade = 1)
    {
        if (!double.IsFinite(hold) || hold < 0) throw new ArgumentOutOfRangeException(nameof(hold));
        if (!double.IsFinite(fade) || fade <= 0) throw new ArgumentOutOfRangeException(nameof(fade));
        return Math.Clamp(1 - (elapsedSeconds - hold) / fade, 0, 1);
    }
}
