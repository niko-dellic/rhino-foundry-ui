using RhinoFoundry.UI.Primitives;
using Xunit;
namespace RhinoFoundry.UI.Primitives.Tests;
public sealed class NoticeTimingTests
{
    [Theory]
    [InlineData(-1, 1, 1)]
    [InlineData(0, 1, 1)]
    [InlineData(0.3, 0, 1)]
    [InlineData(3, 0, 1)]
    [InlineData(3.5, 0, 0.5)]
    [InlineData(4, 0, 0)]
    [InlineData(10, 0, 0)]
    public void NoticeStaysVisibleThenFades(double time, double slide, double opacity)
    {
        Assert.Equal(slide, FoundryNoticeTiming.SlideOffset(time), 6);
        Assert.Equal(opacity, FoundryNoticeTiming.Opacity(time), 6);
    }
    [Fact]
    public void InvalidDurationsAreRejected()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => FoundryNoticeTiming.SlideOffset(0, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => FoundryNoticeTiming.Opacity(0, fade: 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => FoundryNoticeTiming.Opacity(0, hold: -1));
    }
}
