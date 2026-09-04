namespace RhinoFoundry.UI.Primitives;

public sealed class FoundrySelectionModel<TKey> where TKey : struct
{
    private readonly HashSet<TKey> _selected = [];

    public TKey? Anchor { get; private set; }

    public IReadOnlySet<TKey> Selected => _selected;

    public void Replace(IEnumerable<TKey> keys, TKey? anchor = null)
    {
        ArgumentNullException.ThrowIfNull(keys);

        _selected.Clear();
        _selected.UnionWith(keys);
        Anchor = anchor is { } candidate && _selected.Contains(candidate)
            ? candidate
            : _selected.Count == 1
                ? _selected.Single()
                : null;
    }

    public void Toggle(TKey key)
    {
        if (!_selected.Add(key))
        {
            _selected.Remove(key);
        }

        Anchor = _selected.Contains(key) ? key : _selected.FirstOrDefault();
        if (_selected.Count == 0)
        {
            Anchor = null;
        }
    }

    public void SelectRange(
        IReadOnlyList<TKey> visibleOrder,
        TKey target,
        bool additive)
    {
        ArgumentNullException.ThrowIfNull(visibleOrder);

        var targetIndex = IndexOf(visibleOrder, target);
        if (targetIndex < 0)
        {
            return;
        }

        var anchor = Anchor is { } currentAnchor && IndexOf(visibleOrder, currentAnchor) >= 0
            ? currentAnchor
            : target;
        var anchorIndex = IndexOf(visibleOrder, anchor);
        if (!additive)
        {
            _selected.Clear();
        }

        var start = Math.Min(anchorIndex, targetIndex);
        var end = Math.Max(anchorIndex, targetIndex);
        for (var index = start; index <= end; index++)
        {
            _selected.Add(visibleOrder[index]);
        }

        Anchor = anchor;
    }

    public void Prune(IEnumerable<TKey> existingKeys)
    {
        ArgumentNullException.ThrowIfNull(existingKeys);

        _selected.IntersectWith(existingKeys);
        if (Anchor is { } anchor && !_selected.Contains(anchor))
        {
            Anchor = _selected.Count == 1 ? _selected.Single() : null;
        }
    }

    public IReadOnlyList<TKey> VisibleSelection(
        IEnumerable<TKey> visibleKeys)
    {
        ArgumentNullException.ThrowIfNull(visibleKeys);
        return visibleKeys.Where(_selected.Contains).ToArray();
    }

    public void Clear()
    {
        _selected.Clear();
        Anchor = null;
    }

    private static int IndexOf(
        IReadOnlyList<TKey> values,
        TKey target)
    {
        for (var index = 0; index < values.Count; index++)
        {
            if (EqualityComparer<TKey>.Default.Equals(values[index], target))
            {
                return index;
            }
        }

        return -1;
    }
}
