using Eto.Forms;
using Eto.Drawing;
namespace RhinoFoundry.UI;
/// <summary>Native presentation hooks. No service owns a Rhino document.</summary>
public interface IFoundryNativeServices
{
    IDisposable? AttachCanvas(Control control, Func<PointF, bool> isOverlay,
        Action<double, double> pan, Action<double, PointF> zoom);
    IDisposable? AttachClipboardShortcuts(Control scope, Func<bool> canHandle, Action copy, Action paste);
    void ConfigureAlternatingRows(Grid tree);
    void SelectRows(TreeGridView tree, IReadOnlyList<int> rows);
    /// <summary>Show a caller-owned action menu without text-context augmentation. False requests the portable fallback.</summary>
    bool ShowActionMenu(ContextMenu menu, Control anchor) => false;
}
public static class FoundryNative
{
    public static IFoundryNativeServices? Services { get; private set; }
    public static void Register(IFoundryNativeServices services) => Services = services ?? throw new ArgumentNullException(nameof(services));
}
