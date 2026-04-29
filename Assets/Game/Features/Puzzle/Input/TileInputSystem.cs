using UnityEngine;
public interface ITileInputSystem
{
    void Update();
}
public sealed class TileInputSystem : ITileInputSystem
{
    private readonly IPointerInputSystem _pointer;
    private readonly IGridMapper _gridMapper;

    private bool _isDragging;
    private GridPosition _start;

    public TileInputSystem(
        IPointerInputSystem pointer,
        IGridMapper gridMapper)
    {
        _pointer = pointer;
        _gridMapper = gridMapper;
    }

    public void Update()
    {
        var pointer = _pointer.Current;

        if (pointer.Down)
        {
            if (_gridMapper.TryMap(pointer.Position, out var pos))
            {
                _start = pos;
                _isDragging = true;

                // TODO: select tile
            }
        }

        if (_isDragging && pointer.Held)
        {
            if (_gridMapper.TryMap(pointer.Position, out var pos))
            {
                // TODO: drag logic
            }
        }

        if (_isDragging && pointer.Up)
        {
            _isDragging = false;

            // TODO: release / swap / match
        }
    }
}
