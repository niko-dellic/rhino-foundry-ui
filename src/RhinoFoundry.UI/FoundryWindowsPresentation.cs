using System.Collections;
using System.Reflection;
using Eto.Drawing;
using Eto.Forms;

namespace RhinoFoundry.UI;

// WPF is supplied by Rhino, not a portable assembly dependency. Keep the bridge
// private and scoped to opted-in controls; never replace Rhino's application styles.
internal static class FoundryWindowsPresentation
{
    private static Type? Wpf(string name) => Type.GetType(name + ", PresentationFramework");
    private static object Parse(string xaml) => Wpf("System.Windows.Markup.XamlReader")!
        .GetMethod("Parse", new[] { typeof(string) })!.Invoke(null, new object[] { xaml })!;
    private static string Brush(Color color) => $"#{color.Ab:X2}{color.Rb:X2}{color.Gb:X2}{color.Bb:X2}";
    private static void Set(object target, string property, object? value)
    {
        var member = target.GetType().GetProperty(property);
        if (member is { CanWrite: true }) member.SetValue(target, value);
    }
    private static bool Is(object value, string name) => Wpf("System.Windows.Controls." + name)?.IsInstanceOfType(value) == true;

    private static IEnumerable<object> Visuals(object root)
    {
        yield return root;
        var helper = Type.GetType("System.Windows.Media.VisualTreeHelper, PresentationCore")!;
        var count = (int)helper.GetMethod("GetChildrenCount")!.Invoke(null, new[] { root })!;
        var child = helper.GetMethod("GetChild")!;
        for (var index = 0; index < count; index++)
            foreach (var item in Visuals(child.Invoke(null, new object[] { root, index })!))
                yield return item;
    }

    internal static void Field(Control editor)
    {
        if (!OperatingSystem.IsWindows()) return;
        Try(() =>
        {
            var root = editor.ControlObject;
            while (root is Control wrapped && !ReferenceEquals(root, wrapped.ControlObject)) root = wrapped.ControlObject;
            root.GetType().GetMethod("ApplyTemplate")?.Invoke(root, null);
            if (editor is DropDown)
            {
                Set(root, "Template", ComboBoxTemplate());
                root.GetType().GetMethod("ApplyTemplate")?.Invoke(root, null);
            }
            var transparent = Parse("<SolidColorBrush xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' Color='Transparent'/>");
            var zero = Activator.CreateInstance(Wpf("System.Windows.Thickness")!, 0d);
            // The shell owns the border. Retain the real TextBox/ComboBox and
            // spinner buttons, including their automation peers and key handling.
            foreach (var part in Visuals(root))
            {
                if (Is(part, "Border") || Is(part, "Control"))
                {
                    Set(part, "BorderThickness", zero);
                    Set(part, "Background", transparent);
                }
                if (Is(part, "TextBox")) Set(part, "Template", Parse("<ControlTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' TargetType='TextBox'><ScrollViewer x:Name='PART_ContentHost' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' Background='Transparent'/></ControlTemplate>"));
                if (Is(part, "Control")) Set(part, "FocusVisualStyle", null);
                if (Is(part, "Control") || Is(part, "TextBlock"))
                    Set(part, "FontSize", FoundryTheme.FormFont.Size * 96d / 72d);
            }
        });
    }

    private static object ComboBoxTemplate() => Parse($$$"""
        <ControlTemplate xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" TargetType="ComboBox">
          <Grid>
            <ToggleButton Focusable="False" IsChecked="{Binding IsDropDownOpen, Mode=TwoWay, RelativeSource={RelativeSource TemplatedParent}}">
              <ToggleButton.Template><ControlTemplate TargetType="ToggleButton"><Border Background="Transparent"><Path HorizontalAlignment="Right" Margin="0,0,7,0" VerticalAlignment="Center" Data="M 0 0 L 3 3 L 6 0" Stroke="{{{Brush(FoundryTheme.PrimaryText)}}}" StrokeThickness="1.3"/></Border></ControlTemplate></ToggleButton.Template>
            </ToggleButton>
            <ContentPresenter Margin="5,0,24,0" VerticalAlignment="Center" IsHitTestVisible="False" Content="{TemplateBinding SelectionBoxItem}" ContentTemplate="{TemplateBinding SelectionBoxItemTemplate}" ContentTemplateSelector="{TemplateBinding ItemTemplateSelector}"/>
            <Popup x:Name="PART_Popup" Placement="Bottom" IsOpen="{TemplateBinding IsDropDownOpen}" AllowsTransparency="True" Focusable="False">
              <Border Background="{{{Brush(FoundryTheme.InputBackground)}}}" BorderBrush="{{{Brush(FoundryTheme.CanvasBorder)}}}" BorderThickness="1" MinWidth="{Binding ActualWidth, RelativeSource={RelativeSource TemplatedParent}}" MaxHeight="{TemplateBinding MaxDropDownHeight}">
                <ScrollViewer CanContentScroll="True"><ItemsPresenter KeyboardNavigation.DirectionalNavigation="Contained"/></ScrollViewer>
              </Border>
            </Popup>
          </Grid>
          <ControlTemplate.Triggers><Trigger Property="IsEnabled" Value="False"><Setter Property="Opacity" Value="0.5"/></Trigger></ControlTemplate.Triggers>
        </ControlTemplate>
        """);
    internal static void Table(Grid table)
    {
        if (!OperatingSystem.IsWindows()) return;
        Try(() =>
        {
            var grid = Visuals(table.ControlObject).FirstOrDefault(part => Is(part, "DataGrid"));
            if (grid is null) return;
            var background = Brush(FoundryTheme.ContentBackground);
            var alternate = Brush(FoundryTheme.HierarchyAlternateRowBackground);
            var foreground = Brush(FoundryTheme.PrimaryText);
            var border = Brush(FoundryTheme.CanvasBorder);
            var selection = Brush(SystemColors.Selection);
            var selectionText = Brush(SystemColors.SelectionText);
            var resources = (IDictionary)Parse($$"""
                <ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
                  <Style x:Key="Header" TargetType="DataGridColumnHeader">
                    <Setter Property="Background" Value="{{background}}"/><Setter Property="Foreground" Value="{{foreground}}"/>
                    <Setter Property="Padding" Value="8,5"/><Setter Property="BorderBrush" Value="{{border}}"/>
                    <Setter Property="BorderThickness" Value="0,0,0,1"/>
                    <Setter Property="Template"><Setter.Value><ControlTemplate TargetType="DataGridColumnHeader">
                      <Grid><Border Background="{TemplateBinding Background}" BorderBrush="{TemplateBinding BorderBrush}" BorderThickness="{TemplateBinding BorderThickness}" Padding="{TemplateBinding Padding}"><ContentPresenter VerticalAlignment="Center"/></Border><Thumb x:Name="PART_LeftHeaderGripper" Width="4" HorizontalAlignment="Left" Background="Transparent" Cursor="SizeWE"><Thumb.Template><ControlTemplate TargetType="Thumb"><Border Background="Transparent"/></ControlTemplate></Thumb.Template></Thumb><Thumb x:Name="PART_RightHeaderGripper" Width="4" HorizontalAlignment="Right" Background="Transparent" Cursor="SizeWE"><Thumb.Template><ControlTemplate TargetType="Thumb"><Border Background="Transparent"/></ControlTemplate></Thumb.Template></Thumb></Grid>
                    </ControlTemplate></Setter.Value></Setter>
                  </Style>
                  <Style x:Key="Cell" TargetType="DataGridCell">
                    <Setter Property="BorderThickness" Value="0"/><Setter Property="Background" Value="Transparent"/>
                    <Setter Property="Foreground" Value="{{foreground}}"/><Setter Property="FocusVisualStyle" Value="{x:Null}"/>
                    <Setter Property="Template"><Setter.Value><ControlTemplate TargetType="DataGridCell">
                      <Border Background="{TemplateBinding Background}"><ContentPresenter VerticalAlignment="Center"/></Border>
                    </ControlTemplate></Setter.Value></Setter>
                    <Style.Triggers><Trigger Property="IsSelected" Value="True"><Setter Property="Background" Value="{{selection}}"/><Setter Property="Foreground" Value="{{selectionText}}"/></Trigger></Style.Triggers>
                  </Style>
                </ResourceDictionary>
                """);
            Set(grid, "ColumnHeaderStyle", resources["Header"]);
            Set(grid, "FocusVisualStyle", null);
            Set(grid, "BorderBrush", Parse($"<SolidColorBrush xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' Color='{border}'/>"));
            Set(grid, "CellStyle", resources["Cell"]);
            Set(grid, "Background", Parse($"<SolidColorBrush xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' Color='{background}'/>"));
            Set(grid, "RowBackground", Parse($"<SolidColorBrush xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' Color='{background}'/>"));
            Set(grid, "AlternatingRowBackground", Parse($"<SolidColorBrush xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' Color='{alternate}'/>"));
            Set(grid, "AlternationCount", 2);
        });
    }

    private static void Try(Action apply)
    {
        try { apply(); }
        catch (Exception error) when (error is TargetInvocationException or ArgumentException or InvalidOperationException or TypeLoadException or NullReferenceException)
        {
            // A host handler change must degrade to Eto styling, never crash Rhino.
            System.Diagnostics.Trace.WriteLine($"Foundry WPF presentation: {error}");
        }
    }
}
