using AppKit;
using Eto.Mac.Forms.Controls;
using CoreGraphics;
using Foundation;
using Eto.Forms;
using Eto.Drawing;
namespace RhinoFoundry.UI.MacOS;
/// <summary>Call once from a Mac consumer's composition boundary, before creating controls.</summary>
public static class FoundryMacOS
{
    public static void Initialize() => FoundryNative.Register(new Services());
    private sealed class Services : IFoundryNativeServices
    {
        public bool ShowActionMenu(ContextMenu menu, Control anchor)
        {
            if (menu.ControlObject is not NSMenu nativeMenu || MacOSHelpers.ToNative(anchor, false) is not { } view)
                return false;
            // A toolbar action popup is not a text context menu. Avoid PopUpContextMenu,
            // which can inject AutoFill when Eto's drawable advertises text-input support.
            var previous = nativeMenu.AllowsContextMenuPlugIns;
            try
            {
                nativeMenu.AllowsContextMenuPlugIns = false;
                nativeMenu.PopUpMenu(null, new CoreGraphics.CGPoint(0, view.IsFlipped ? view.Bounds.Height : 0), view);
            }
            finally { nativeMenu.AllowsContextMenuPlugIns = previous; }
            return true;
        }
        public IDisposable AttachCanvas(Control control, Func<PointF,bool> overlay, Action<double,double> pan, Action<double,PointF> zoom) => new Gestures(control, overlay, pan, zoom);
        public IDisposable AttachClipboardShortcuts(Control scope, Func<bool> canHandle, Action copy, Action paste) =>
            new ClipboardShortcuts(scope, canHandle, copy, paste);
        public void ConfigureAlternatingRows(Grid tree)
        { if (FindTable(MacOSHelpers.ToNative(tree, false)) is { } view) view.UsesAlternatingRowBackgroundColors = true; }
        public void ConfigureSelectionColor(TreeGridView tree, Func<Color> color)
        {
            if (Find(MacOSHelpers.ToNative(tree, false)) is not { } view) return;
            if (view.Delegate is SelectionOutlineDelegate current) { current.Color = color; return; }
            if (view.Delegate is not TreeGridViewHandler.EtoOutlineDelegate original) return;
            var selection = new SelectionOutlineDelegate { Handler = original.Handler, Color = color };
            // Eto's delegate retains all editing, hierarchy, drag and selection behavior.
            // Keep our specialization alive for the same lifetime as the widget.
            tree.Properties["Foundry.SelectionDelegate"] = selection;
            view.Delegate = selection;
            view.ReloadData();
        }
        public void SelectRows(TreeGridView tree, IReadOnlyList<int> rows)
        {
            if (Find(MacOSHelpers.ToNative(tree, false)) is not { } view) return;
            using var indices = new NSMutableIndexSet();
            foreach (var row in rows) if (row >= 0) indices.Add((nuint)row);
            view.SelectRows(indices, false);
        }
        private static NSTableView? FindTable(NSView? view)
        {
            if (view is NSTableView table) return table;
            if (view is null) return null;
            foreach (var child in view.Subviews) if (FindTable(child) is { } found) return found;
            return null;
        }
        private static NSOutlineView? Find(NSView? view)
        {
            if (view is NSOutlineView outline) return outline;
            if (view is null) return null;
            foreach (var child in view.Subviews) if (Find(child) is { } found) return found;
            return null;
        }
    }
    private sealed class SelectionOutlineDelegate : TreeGridViewHandler.EtoOutlineDelegate
    {
        internal Func<Color> Color { get; set; } = null!;
        public override NSTableRowView RowViewForItem(NSOutlineView outlineView, NSObject item) =>
            new SelectionRowView(() => Color());
    }

    private sealed class SelectionRowView(Func<Color> color) : NSTableRowView
    {
        public override NSBackgroundStyle InteriorBackgroundStyle =>
            !Selected ? base.InteriorBackgroundStyle :
            FoundryTable.SelectionForeground(color()) == Colors.White ? NSBackgroundStyle.Emphasized : NSBackgroundStyle.Normal;

        public override void DrawSelection(CGRect dirtyRect)
        {
            var value = color();
            using var fill = NSColor.FromRgba(value.R, value.G, value.B, 1f);
            fill.SetFill();
            using var path = NSBezierPath.FromRoundedRect(new CGRect(0, 1, Bounds.Width, Math.Max(0, Bounds.Height - 2)), 4, 4);
            path.Fill();
        }
    }

    private sealed class ClipboardShortcuts : IDisposable
    {
        private readonly Control _scope;
        private readonly NSView _view;
        private readonly Func<bool> _canHandle;
        private readonly Action _copy;
        private readonly Action _paste;
        private NSObject? _monitor;

        public ClipboardShortcuts(Control scope, Func<bool> canHandle, Action copy, Action paste)
        {
            _scope = scope;
            _view = MacOSHelpers.ToNative(scope, false);
            _canHandle = canHandle;
            _copy = copy;
            _paste = paste;
            _monitor = NSEvent.AddLocalMonitorForEventsMatchingMask(NSEventMask.KeyDown, Handle);
        }

        private NSEvent Handle(NSEvent e)
        {
            if (_scope.IsDisposed || !_scope.Visible || e.Window is null || e.Window != _view.Window || !_canHandle() ||
                e.Window.FirstResponder is not NSView responder || responder is NSTextView or NSTextField)
                return e;
            var inside = false;
            for (var view = responder; view is not null; view = view.Superview)
                if (ReferenceEquals(view, _view)) { inside = true; break; }
            if (!inside) return e;
            var modifiers = e.ModifierFlags;
            if (!(modifiers.HasFlag(NSEventModifierMask.CommandKeyMask) || modifiers.HasFlag(NSEventModifierMask.ControlKeyMask)) ||
                modifiers.HasFlag(NSEventModifierMask.AlternateKeyMask) || modifiers.HasFlag(NSEventModifierMask.ShiftKeyMask)) return e;
            if (string.Equals(e.CharactersIgnoringModifiers, "c", StringComparison.OrdinalIgnoreCase)) _copy();
            else if (string.Equals(e.CharactersIgnoringModifiers, "v", StringComparison.OrdinalIgnoreCase)) _paste();
            else return e;
            // AppKit otherwise routes Command-C/V through Rhino's menu before Eto KeyDown.
            return null!;
        }

        public void Dispose()
        {
            if (_monitor is null) return;
            NSEvent.RemoveMonitor(_monitor);
            _monitor.Dispose();
            _monitor = null;
        }
    }

    private sealed class Gestures : IDisposable
    {
        private readonly NSView _view;
        private readonly Control _control;
        private readonly Func<PointF,bool> _overlay;
        private readonly Action<double,double> _pan;
        private readonly Action<double,PointF> _zoom;
        private NSObject? _scroll;
        private NSObject? _magnify;
        public Gestures(Control control, Func<PointF,bool> overlay, Action<double,double> pan, Action<double,PointF> zoom)
        {
            _control=control; _view=MacOSHelpers.ToNative(control,false); _overlay=overlay; _pan=pan; _zoom=zoom;
            try
            {
                _scroll = NSEvent.AddLocalMonitorForEventsMatchingMask(NSEventMask.ScrollWheel, Scroll);
                _magnify = NSEvent.AddLocalMonitorForEventsMatchingMask(NSEventMask.EventMagnify, Magnify);
            }
            catch { Dispose(); throw; }
        }
        private bool Point(NSEvent e, out PointF point)
        {
            point=default;
            if (_control.IsDisposed || !_control.Visible || e.Window is null || e.Window != _view.Window) return false;
            var p=_view.ConvertPointFromView(e.LocationInWindow,null);
            if (!_view.Bounds.Contains(p)) return false;
            point=new((float)p.X, (float)(_view.IsFlipped?p.Y:_view.Bounds.Height-p.Y));
            return !_overlay(point);
        }
        private NSEvent Scroll(NSEvent e)
        {
            if (!e.HasPreciseScrollingDeltas || !Point(e,out _)) return e;
            // Preserve AppKit system-mapped direction and Foundry's screen translation convention.
            _pan(-(double)e.ScrollingDeltaX,-(double)e.ScrollingDeltaY); return null!;
        }
        private NSEvent Magnify(NSEvent e)
        {
            if (!Point(e,out var p)) return e;
            if (Math.Abs((double)e.Magnification)>=double.Epsilon) _zoom(Math.Exp((double)e.Magnification),p);
            return null!;
        }
        public void Dispose()
        {
            if (_scroll is not null) { NSEvent.RemoveMonitor(_scroll); _scroll.Dispose(); _scroll=null; }
            if (_magnify is not null) { NSEvent.RemoveMonitor(_magnify); _magnify.Dispose(); _magnify=null; }
        }
    }
}
