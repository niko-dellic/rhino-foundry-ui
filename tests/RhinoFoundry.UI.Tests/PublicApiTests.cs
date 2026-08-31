using Eto.Drawing;
using Xunit;

namespace RhinoFoundry.UI.Tests;

public sealed class PublicApiTests
{
    [Fact]
    public void CoreControlTypesArePublic()
    {
        Type[] publicTypes =
        [
            typeof(FoundryTheme),
            typeof(FoundryDialogButton),
            typeof(FoundryToolbarIconButton),
            typeof(FoundryToolbarButtonGroup),
            typeof(FoundryFormField),
            typeof(FoundryToolbarField),
            typeof(FoundryCheckBox),
            typeof(FoundryColorField),
            typeof(FoundrySlider),
            typeof(FoundryDialogActions),
        ];

        Assert.All(publicTypes, type => Assert.True(type.IsPublic, $"{type.FullName} must remain public."));
    }

    [Fact]
    public void WithAlphaPreservesRgbAndClampsAlpha()
    {
        var source = Color.FromArgb(12, 34, 56, 78);

        var low = FoundryTheme.WithAlpha(source, -10);
        var high = FoundryTheme.WithAlpha(source, 300);

        Assert.Equal((byte)12, low.Rb);
        Assert.Equal((byte)34, low.Gb);
        Assert.Equal((byte)56, low.Bb);
        Assert.Equal((byte)0, low.Ab);
        Assert.Equal((byte)255, high.Ab);
    }
}
