using Eto.Drawing;
using Eto.Forms;
using RhinoFoundry.UI.Primitives;
namespace RhinoFoundry.UI;

/// <summary>Reusable drawing surface with camera math, coalesced input and owned native gestures.
/// Consumers draw through Paint and supply occupied overlay bounds; world units are consumer-defined.</summary>
public class FoundryCanvas : Drawable
{
    private readonly UITimer _frame = new() { Interval = 1d / 60d };
    private readonly UITimer _settle = new() { Interval = 0.08 };
    private FoundryPoint _pendingPan;
    private FoundryPoint _anchor;
    private double _pendingZoom = 1;
    private bool _queued;
    private IDisposable? _native;
    public FoundryCamera CanvasCamera { get; set; } = FoundryCamera.Default;
    protected bool UseDefaultWheelZoom { get; set; } = true;
    public double PendingZoomFactor => _pendingZoom;
    public event EventHandler? CameraChanged;
    public event EventHandler? CameraSettled;
    public FoundryCanvas(bool largeCanvas = true) : base(largeCanvas)
    {
        CanFocus = true;
        _frame.Elapsed += (_, _) => FlushQueuedCameraInput();
        _settle.Elapsed += (_, _) => { _settle.Stop(); OnCameraSettled(); };
        MouseWheel += (_, e) =>
        {
            if (!UseDefaultWheelZoom || IsCanvasOverlay(e.Location)) return;
            QueueCameraZoom(Math.Exp(e.Delta.Height * 0.115), new(e.Location.X, e.Location.Y));
            e.Handled = true;
        };
        LoadComplete += (_, _) =>
        {
            _native?.Dispose();
            _native = FoundryNative.Services?.AttachCanvas(this, IsCanvasOverlay, QueueCameraPan,
                (factor, point) => QueueCameraZoom(factor, new(point.X, point.Y)));
        };
        UnLoad += (_, _) => { _native?.Dispose(); _native = null; StopQueuedCameraInput(); };

    }
    protected override void Dispose(bool disposing)
    {
        if (disposing) { _native?.Dispose(); _native = null; _frame.Dispose(); _settle.Dispose(); }
        base.Dispose(disposing);
    }
    protected virtual bool IsCanvasOverlay(PointF point) => false;
    protected virtual void OnCameraFrame() { Invalidate(); CameraChanged?.Invoke(this, EventArgs.Empty); }
    protected virtual void OnCameraSettled() => CameraSettled?.Invoke(this, EventArgs.Empty);
    protected virtual void ApplyCameraInput(FoundryPoint pan, double zoom, FoundryPoint anchor)
    {
        if (pan.X != 0 || pan.Y != 0) CanvasCamera = CanvasCamera.PanScreen(pan.X, pan.Y);
        if (zoom != 1) CanvasCamera = CanvasCamera.ZoomAt(anchor, zoom, new(Math.Max(1, Width), Math.Max(1, Height)));
    }
    protected void QueueCameraPan(double x, double y)
    {
        if (Math.Abs(x) < double.Epsilon && Math.Abs(y) < double.Epsilon) return;
        _pendingPan += new FoundryPoint(x,y); Schedule();
    }
    protected void QueueCameraZoom(double factor, FoundryPoint anchor)
    {
        if (!double.IsFinite(factor) || factor <= 0 || Math.Abs(factor-1) < double.Epsilon) return;
        _pendingZoom *= factor; _anchor = anchor; Schedule();
    }
    private void Schedule() { if (_queued) return; _queued = true; _frame.Start(); }
    private void FlushQueuedCameraInput()
    {
        _frame.Stop(); _queued = false;
        var pan = _pendingPan; var zoom = _pendingZoom; var anchor = _anchor;
        _pendingPan = new(); _pendingZoom = 1;
        if (pan.X == 0 && pan.Y == 0 && zoom == 1) return;
        ApplyCameraInput(pan, zoom, anchor); OnCameraFrame(); _settle.Stop(); _settle.Start();
    }
    protected void StopQueuedCameraInput()
    {
        _frame.Stop(); _settle.Stop(); _queued = false; _pendingPan = new(); _pendingZoom = 1;
    }
}
