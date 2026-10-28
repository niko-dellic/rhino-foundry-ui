using Eto.Forms;

namespace RhinoFoundry.UI;

/// <summary>Refreshes presentation on load and settled native color changes. Create and dispose on the UI thread.</summary>
/// <remarks>Uses one shared timer with weak registrations. Dispose with the owner; unload suspends callbacks.</remarks>
public sealed class FoundryThemeBinding : IDisposable
{
    private static readonly List<WeakReference<FoundryThemeBinding>> Active = [];
    private static UITimer? _timer;
    private readonly Control _owner;
    private readonly Action _refresh;
    private (bool Dark, int Text, int Background)? _colors;
    private bool _active;
    private bool _disposed;

    public FoundryThemeBinding(Control owner, Action refresh)
    {
        _owner = owner ?? throw new ArgumentNullException(nameof(owner));
        _refresh = refresh ?? throw new ArgumentNullException(nameof(refresh));
        owner.LoadComplete += OnLoad;
        owner.UnLoad += OnUnload;
        if (owner.Loaded) OnLoad(owner, EventArgs.Empty);
    }

    private void OnLoad(object? sender, EventArgs args)
    {
        if (_disposed || _active) return;
        _active = true;
        _colors = null;
        Active.Add(new(this));
        if (_timer is null)
        {
            _timer = new UITimer { Interval = 0.5 };
            _timer.Elapsed += Tick;
        }
        _timer.Start();
        Update();
    }

    private void OnUnload(object? sender, EventArgs args)
    {
        _active = false;
        Prune();
    }

    private static void Prune()
    {
        Active.RemoveAll(reference => !reference.TryGetTarget(out var binding) ||
            !binding._active || binding._disposed || binding._owner.IsDisposed);
        if (Active.Count == 0 && _timer is not null)
        {
            _timer.Stop();
            _timer.Elapsed -= Tick;
            _timer.Dispose();
            _timer = null;
        }
    }

    private static void Tick(object? sender, EventArgs args)
    {
        foreach (var reference in Active.ToArray())
            if (reference.TryGetTarget(out var binding))
            {
                try { binding.Update(); }
                catch (Exception error) { System.Diagnostics.Trace.TraceError("Foundry theme refresh: {0}", error); }
            }
        Prune();
    }

    private void Update()
    {
        if (!_active || _disposed || _owner.IsDisposed) return;
        var colors = (FoundryTheme.IsDarkMode, FoundryTheme.PrimaryText.ToArgb(), FoundryTheme.PanelBackground.ToArgb());
        if (_colors == colors) return;
        // Text and background can change on different AppKit event-loop turns.
        _colors = colors;
        _refresh();
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _owner.LoadComplete -= OnLoad;
        _owner.UnLoad -= OnUnload;
        _active = false;
        Prune();
    }
}
