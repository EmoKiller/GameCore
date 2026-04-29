using UnityEngine;

public sealed class InputSystem 
{
    private readonly IPuzzleSystem _puzzle;

    private Vector2Int? _selected;

    public InputSystem(IPuzzleSystem puzzle)
    {
        _puzzle = puzzle;
    }

    public void Update(float deltaTime)
    {
        if (!UnityEngine.Input.GetMouseButtonDown(0))
            return;

        var pos = UnityEngine.Camera.main.ScreenToWorldPoint(UnityEngine.Input.mousePosition);

        int x = Mathf.RoundToInt(pos.x);
        int y = Mathf.RoundToInt(pos.y);

        if (_selected == null)
        {
            _selected = new Vector2Int(x, y);
        }
        else
        {
            var from = _selected.Value;

            _puzzle.EnqueueSwap(new SwapCommand(from.x, from.y, x, y));

            _selected = null;
        }
    }
}