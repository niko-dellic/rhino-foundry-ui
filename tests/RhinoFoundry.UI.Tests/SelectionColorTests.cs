using Eto.Drawing;
using Xunit;
namespace RhinoFoundry.UI.Tests;
public sealed class SelectionColorTests
{
    [Theory]
    [InlineData(37, 99, 235, false)]
    [InlineData(0, 255, 255, true)]
    [InlineData(255, 255, 255, true)]
    [InlineData(0, 0, 0, false)]
    public void SelectionForegroundContrastsWithChosenColor(int red, int green, int blue, bool black)
    {
        var color = new Color(red / 255f, green / 255f, blue / 255f);
        Assert.Equal(black ? Colors.Black : Colors.White, FoundryTable.SelectionForeground(color));
    }
}
