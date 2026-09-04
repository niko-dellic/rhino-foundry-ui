using RhinoFoundry.UI.Primitives;
using Xunit;

namespace RhinoFoundry.UI.Primitives.Tests;

public sealed class FoundryCameraTests
{
    [Fact]
    public void WorldScreenTransformsRoundTripAtExtremeZooms()
    {
        var viewport = new FoundrySize(1200, 800);
        var world = new FoundryPoint(1523.25, -804.5);
        foreach (var zoom in new[] { FoundryCamera.MinimumZoom, 0.5, 1d, FoundryCamera.MaximumZoom })
        {
            var camera = new FoundryCamera(new FoundryPoint(250, -100), zoom);
            var restored = camera.ScreenToWorld(camera.WorldToScreen(world, viewport), viewport);
            Assert.True(Math.Abs(world.X - restored.X) < 1e-8);
            Assert.True(Math.Abs(world.Y - restored.Y) < 1e-8);
        }
    }

    [Fact]
    public void PointerCenteredZoomKeepsAnchorStable()
    {
        var viewport = new FoundrySize(900, 600);
        var anchor = new FoundryPoint(127, 441);
        var camera = new FoundryCamera(new FoundryPoint(100, 50), 0.75);
        var before = camera.ScreenToWorld(anchor, viewport);

        var zoomed = camera.ZoomAt(anchor, 1.8, viewport);

        var after = zoomed.ScreenToWorld(anchor, viewport);
        Assert.True(Math.Abs(before.X - after.X) < 1e-8);
        Assert.True(Math.Abs(before.Y - after.Y) < 1e-8);
    }

    [Fact]
    public void ReverseDirectionSelectionRectangleProducesNormalizedScreenBounds()
    {
        var camera = new FoundryCamera(new FoundryPoint(0, 0), 2);
        var screen = camera.WorldToScreen(
            new FoundryRect(100, 80, -60, -30),
            new FoundrySize(800, 600));

        Assert.True(screen.Width > 0);
        Assert.True(screen.Height > 0);
        Assert.Equal(120d, screen.Width);
        Assert.Equal(60d, screen.Height);
    }

    [Fact]
    public void ReverseDirectionSelectionRectangleUsesNormalizedContainment()
    {
        var selection = new FoundryRect(100, 100, -80, -60);

        Assert.True(selection.Contains(new FoundryRect(30, 50, 40, 30)));
        Assert.False(selection.Contains(new FoundryRect(10, 50, 40, 30)));
    }

    [Fact]
    public void FitContainsBoundsInsidePaddedViewport()
    {
        var bounds = new FoundryRect(100, 200, 1200, 700);
        var viewport = new FoundrySize(1000, 700);
        var camera = FoundryCamera.Fit(bounds, viewport, 50);
        var screen = camera.WorldToScreen(bounds, viewport);

        Assert.True(screen.Left >= 49.9);
        Assert.True(screen.Top >= 49.9);
        Assert.True(screen.Right <= 950.1);
        Assert.True(screen.Bottom <= 650.1);
    }

    [Fact]
    public void MaximumZoomKeepsSheetCenteredAndPreservesItsAspectRatio()
    {
        var viewport = new FoundrySize(1600, 1000);
        var sheet = new FoundryRect(100, 200, 594, 420);
        var cameraCenter = sheet.Center;
        var normal = new FoundryCamera(cameraCenter, 1);
        var maximum = new FoundryCamera(cameraCenter, FoundryCamera.MaximumZoom);

        var normalScreen = normal.WorldToScreen(sheet, viewport);
        var maximumScreen = maximum.WorldToScreen(sheet, viewport);

        Assert.Equal(viewport.Width / 2, maximumScreen.Center.X, 8);
        Assert.Equal(viewport.Height / 2, maximumScreen.Center.Y, 8);
        Assert.Equal(normalScreen.Width * FoundryCamera.MaximumZoom, maximumScreen.Width, 8);
        Assert.Equal(normalScreen.Height * FoundryCamera.MaximumZoom, maximumScreen.Height, 8);
        Assert.Equal(sheet.Width / sheet.Height, maximumScreen.Width / maximumScreen.Height, 8);
    }
}
