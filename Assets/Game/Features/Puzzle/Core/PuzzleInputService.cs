using Game.Application.Core;
using UnityEngine;
public interface IPuzzleInputService : IService
{
    
}
public sealed class PuzzleInputService : IPuzzleInputService , IUpdatable
{
    [SerializeField]
    private Camera _camera;

    private IPuzzleService _service;

    private TileView _selectedTile;
    private Vector2 _pointerDownPosition;
    public PuzzleInputService(IPuzzleService service)
    {
        _service = service;
    }
    public void OnUpdate(float deltaTime)
    {
        Update();
    }
    private void Update()
    {
        
        if (Input.GetMouseButtonDown(0))
        {
            HandlePointerDown();
        }

        if (Input.GetMouseButtonUp(0))
        {
            HandlePointerUp();
        }
    }
    private void HandlePointerDown()
    {
        _pointerDownPosition = Input.mousePosition;
        _selectedTile = RaycastTile();
    }
    private TileView RaycastTile()
    {
        Vector3 world =
            _camera.ScreenToWorldPoint(
                Input.mousePosition);

        Vector2 position = new Vector2(
            world.x,
            world.y);

        RaycastHit2D hit =
            Physics2D.Raycast(
                position,
                Vector2.zero);

        if (hit.collider == null)
        {
            return null;
        }

        return hit.collider.GetComponent<TileView>();
    }
    private void HandlePointerUp()
    {
        if (_selectedTile == null)
        {
            return;
        }

        Vector2 dragDelta =
            (Vector2)Input.mousePosition -
            _pointerDownPosition;

        if (dragDelta.magnitude < 20f)
        {
            _selectedTile = null;
            return;
        }

        Vector2Int direction =
            GetSwapDirection(dragDelta);

        ExecuteSwap(direction);

        _selectedTile = null;
    }
    private Vector2Int GetSwapDirection(
        Vector2 dragDelta)
    {
        if (Mathf.Abs(dragDelta.x) >
            Mathf.Abs(dragDelta.y))
        {
            return dragDelta.x > 0
                ? Vector2Int.right
                : Vector2Int.left;
        }

        return dragDelta.y > 0
            ? Vector2Int.up
            : Vector2Int.down;
    }
    private void ExecuteSwap(
        Vector2Int direction)
    {
        TilePosition from =
            _selectedTile.Position;

        TilePosition to =
            new TilePosition(
                from.X + direction.x,
                from.Y + direction.y);

        SwapResult swapResult =
            _service.TrySwap(from, to);

        Debug.Log($"Swap Result: {swapResult.Success}");
    }


}
