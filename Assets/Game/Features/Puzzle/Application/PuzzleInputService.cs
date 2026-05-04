using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Application.Core;
using UnityEngine;
using UnityEngine.InputSystem;
public interface IPuzzleInputService : IService
{
    
}
public sealed class PuzzleInputService : IPuzzleInputService , IUpdatable , IOnPreShutdown
{   
    private const float SwipeThreshold = 30f;
    private readonly Camera _camera;
    private readonly IPointerInputReader _input;
    private readonly IPuzzleGameplayService _service;

    private TilePosition? _selectedTile;
    private Vector2 _pointerDownPosition;
    private bool _hasConsumedSwipe;

    private readonly CancellationTokenSource _disposeCts = new();
    private CancellationToken Token => _disposeCts.Token;

    public PuzzleInputService(
        IPuzzleGameplayService service,
        IPointerInputReader input,
        Camera camera)
    {
        _service = service;
        _input = input;
        _camera = camera;
    }
    public void OnUpdate(float deltaTime)
    {
        ProcessInput();
    }
    private void ProcessInput()
    {
        if (_service.IsBusy)
        {
            return;
        }

        if (_input.WasPressedThisFrame())
        {
            HandlePointerDown();
        }

        if (_input.IsPressed())
        {
            HandleDragging();
        }
    }

    private void HandlePointerDown()
    {
        _pointerDownPosition =
            _input.GetPosition();

        _selectedTile =
            RaycastTile(
                _pointerDownPosition);

        _hasConsumedSwipe = false;
    }

    private void HandleDragging()
    {
        if (_selectedTile.HasValue == false)
        {
            return;
        }

        if (_hasConsumedSwipe)
        {
            return;
        }

        Vector2 currentPosition =
            _input.GetPosition();

        Vector2 dragDelta =
            currentPosition -
            _pointerDownPosition;

        if (dragDelta.magnitude < SwipeThreshold)
        {
            return;
        }

        Vector2Int direction =
            GetSwapDirection(
                dragDelta);

        _hasConsumedSwipe = true;

        ExecuteSwap(direction).Forget();
    }

    private TilePosition? RaycastTile(
        Vector2 screenPosition)
    {
        Vector3 screenPoint =
            new Vector3(
                screenPosition.x,
                screenPosition.y,
                -_camera.transform.position.z);

        Vector3 world =
            _camera.ScreenToWorldPoint(
                screenPoint);

        Vector2 worldPosition =
            new Vector2(
                world.x,
                world.y);

        RaycastHit2D hit =
            Physics2D.Raycast(
                worldPosition,
                Vector2.zero);

        if (hit.collider == null)
        {
            return null;
        }

        TileView view =
            hit.collider.GetComponent<TileView>();

        if (view == null)
        {
            return null;
        }

        return view.Position;
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

    private async UniTask ExecuteSwap(Vector2Int direction)
    {
        try
        {
            if (_selectedTile.HasValue == false)
            {
                return;
            }

            TilePosition from =
                _selectedTile.Value;

            TilePosition to =
                new TilePosition(
                    from.X + direction.x,
                    from.Y + direction.y);

            if (_service.IsInside(to) == false)
            {
                return;
            }

            await _service.TrySwapAsync(
                from,
                to,
                Token);

            _selectedTile = null;
        }
        catch (OperationCanceledException)
        {
        }
    }

    public void OnPreShutdown()
    {
        _disposeCts.Cancel();
        _disposeCts.Dispose();
    }
}
