// using UnityEngine;

// public sealed class PuzzleDragPreviewController
// {
//     private readonly PuzzleBoardView _boardView;

//     private readonly float _maxPreviewDistance;

//     private TileView _selectedView;

//     private TileView _neighborView;

//     private TilePosition _selectedPosition;

//     private Vector2Int _currentDirection;

//     private bool _isDragging;

//     public PuzzleDragPreviewController(
//         PuzzleBoardView boardView,
//         float maxPreviewDistance = 0.8f)
//     {
//         _boardView = boardView;

//         _maxPreviewDistance = maxPreviewDistance;
//     }

//     public void Begin(
//         TilePosition position)
//     {
//         _selectedPosition = position;

//         _selectedView =
//             _boardView.GetTileView(position);

//         if (_selectedView == null)
//         {
//             return;
//         }

//         _neighborView = null;

//         _currentDirection = Vector2Int.zero;

//         _isDragging = true;
//     }

//     public void UpdateDrag(
//         Vector2 dragDelta,
//         float tileSize)
//     {
//         if (_isDragging == false)
//         {
//             return;
//         }

//         if (_selectedView == null)
//         {
//             return;
//         }

//         Vector2Int direction =
//             GetDirection(dragDelta);

//         if (direction == Vector2Int.zero)
//         {
//             _selectedView.ResetPreview();

//             ResetNeighborPreview();

//             return;
//         }

//         if (_currentDirection != direction)
//         {
//             ResetNeighborPreview();

//             _currentDirection = direction;

//             SetupNeighbor(direction);
//         }

//         float dragAmount =
//             GetDirectionalMagnitude(
//                 dragDelta,
//                 direction);

//         float maxPixelDistance = 100f;

//         float clamped =
//             Mathf.Clamp01(
//                 dragAmount /
//                 maxPixelDistance);

//         float worldDistance =
//             clamped *
//             tileSize *
//             _maxPreviewDistance;

//         Vector3 offset =
//             new Vector3(
//                 direction.x,
//                 direction.y,
//                 0f) * worldDistance;

//         _selectedView.SetPreviewOffset(offset);

//         if (_neighborView != null)
//         {
//             _neighborView.SetPreviewOffset(
//                 -offset);
//         }
//     }

//     public void End()
//     {
//         if (_selectedView != null)
//         {
//             _selectedView.ResetPreview();
//         }

//         ResetNeighborPreview();

//         _isDragging = false;
//     }

//     private void SetupNeighbor(
//         Vector2Int direction)
//     {
//         TilePosition neighborPosition =
//             new TilePosition(
//                 _selectedPosition.X + direction.x,
//                 _selectedPosition.Y + direction.y);

//         if (_boardView.IsInside(neighborPosition) == false)
//         {
//             _neighborView = null;

//             return;
//         }

//         _neighborView =
//             _boardView.GetTileView(
//                 neighborPosition);
//     }

//     private void ResetNeighborPreview()
//     {
//         if (_neighborView != null)
//         {
//             _neighborView.ResetPreview();

//             _neighborView = null;
//         }
//     }

//     private Vector2Int GetDirection(
//         Vector2 delta)
//     {
//         if (delta.magnitude < 10f)
//         {
//             return Vector2Int.zero;
//         }

//         if (Mathf.Abs(delta.x) >
//             Mathf.Abs(delta.y))
//         {
//             return delta.x > 0
//                 ? Vector2Int.right
//                 : Vector2Int.left;
//         }

//         return delta.y > 0
//             ? Vector2Int.up
//             : Vector2Int.down;
//     }

//     private float GetDirectionalMagnitude(
//         Vector2 delta,
//         Vector2Int direction)
//     {
//         if (direction.x > 0)
//         {
//             return Mathf.Max(0f, delta.x);
//         }

//         if (direction.x < 0)
//         {
//             return Mathf.Max(0f, -delta.x);
//         }

//         if (direction.y > 0)
//         {
//             return Mathf.Max(0f, delta.y);
//         }

//         return Mathf.Max(0f, -delta.y);
//     }
// }