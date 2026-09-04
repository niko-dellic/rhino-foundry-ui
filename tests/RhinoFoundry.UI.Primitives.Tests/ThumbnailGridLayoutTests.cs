using RhinoFoundry.UI.Primitives;
using Xunit;

namespace RhinoFoundry.UI.Primitives.Tests;

public sealed class FoundryThumbnailGridLayoutTests
{
    [Fact]
    public void LargerRequestedCardsProduceFewerColumns()
    {
        var compact = FoundryThumbnailGridLayout.Create(40, 1200, 140);
        var large = FoundryThumbnailGridLayout.Create(40, 1200, 300);

        Assert.True(compact.Columns > large.Columns);
        Assert.Equal(40, compact.ItemCount);
        Assert.Equal(40, large.ItemCount);
    }

    [Fact]
    public void FullWidthRequestAlwaysProducesOneItemPerRow()
    {
        var layout = FoundryThumbnailGridLayout.Create(40, 3840, 3840);

        Assert.Equal(1, layout.Columns);
        Assert.Equal(40, layout.Rows);
    }

    [Fact]
    public void DensityDistributesColumnCountsAcrossTheEntireControlRange()
    {
        var compact = FoundryThumbnailGridLayout.CreateForDensity(40, 3840, 0);
        var midpoint = FoundryThumbnailGridLayout.CreateForDensity(40, 3840, 0.5);
        var nearLargest = FoundryThumbnailGridLayout.CreateForDensity(40, 3840, 0.75);
        var largest = FoundryThumbnailGridLayout.CreateForDensity(40, 3840, 1);

        Assert.True(compact.Columns > midpoint.Columns);
        Assert.InRange(midpoint.Columns, compact.Columns / 2, compact.Columns / 2 + 1);
        Assert.True(nearLargest.Columns > 1);
        Assert.Equal(1, largest.Columns);
    }

    [Fact]
    public void GridFillsAvailableWidthWithoutOverlappingCells()
    {
        var layout = FoundryThumbnailGridLayout.Create(12, 1000, 210);
        var first = layout.CellBounds(0);
        var second = layout.CellBounds(1);

        Assert.True(second.X >= first.X + first.Width + layout.Gap - 0.001);
        var lastColumn = layout.CellBounds(layout.Columns - 1);
        Assert.True(lastColumn.X + lastColumn.Width <= 1000 - layout.Padding + 0.001);
    }

    [Fact]
    public void LandscapeRowsReserveOnlyTheirRenderedPaperHeight()
    {
        var layout = FoundryThumbnailGridLayout.CreateForDensity(
            [420d / 594d, 420d / 594d],
            1200,
            1);
        var first = layout.CellBounds(0);
        var second = layout.CellBounds(1);

        Assert.Equal(1, layout.Columns);
        Assert.InRange(second.Y - first.Bottom, layout.Gap - 0.001, layout.Gap + 0.001);
        Assert.True(first.Height < layout.CardWidth * 0.78 + 42);
    }

    [Fact]
    public void EachRowUsesItsTallestPaperAndVariableOffsetsRemainHittable()
    {
        var layout = FoundryThumbnailGridLayout.CreateForDensity(
            [420d / 594d, 594d / 420d, 420d / 594d],
            300,
            0);
        var firstRow = layout.CellBounds(0);
        var secondRow = layout.CellBounds(2);

        Assert.Equal(2, layout.Columns);
        Assert.True(firstRow.Height > secondRow.Height);
        Assert.Equal(0, layout.RowAt(firstRow.Y + 1));
        Assert.Equal(1, layout.RowAt(secondRow.Y + 1));
    }

    [Fact]
    public void VisibleQueryReturnsOnlyViewportRowsAndOverscan()
    {
        var layout = FoundryThumbnailGridLayout.Create(100, 900, 180);
        var visible = layout.VisibleIndices(
            layout.RowHeight * 4,
            layout.RowHeight * 5,
            overscanRows: 1);

        Assert.True(visible.Count < layout.ItemCount);
        Assert.True(visible.All(index => index >= layout.Columns * 2));
        Assert.True(visible.All(index => index < layout.Columns * 7));
        Assert.Empty(layout.VisibleIndices(layout.ContentHeight + 100, layout.ContentHeight + 300));
    }
}
