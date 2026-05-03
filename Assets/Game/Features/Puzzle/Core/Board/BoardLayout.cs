using UnityEngine;

public interface IReadOnlyBoardLayout
{
    float TileSize{ get; }
    Vector3 GetWorldPosition(TilePosition position);
    Vector3 GetSpawnWorldPosition(int column, IReadOnlyPuzzleBoard board);
}

public sealed class BoardLayout : IReadOnlyBoardLayout
{
    public float TileSize => _tileSize;

    private readonly Vector2 _origin;

    private readonly float _tileSize;

    private readonly int _width;

    private readonly int _height;

    public BoardLayout(
        int width,
        int height,
        Camera camera,
        float horizontalPadding = 0.9f,
        float verticalPadding = 0.9f,
        float minTileSize = 0.9f,
        float maxTileSize = 1.1f,
        Vector2 boardOffset = default)
    {
        _width = width;
        _height = height;

        float screenHeight =
            camera.orthographicSize * 2f;

        float screenWidth =
            screenHeight *
            Screen.width /
            Screen.height;

        float availableWidth =
            screenWidth * horizontalPadding;

        float availableHeight =
            screenHeight * verticalPadding;

        float tileWidth =
            availableWidth / width;

        float tileHeight =
            availableHeight / height;

        float calculatedTileSize =
            Mathf.Min(
                tileWidth,
                tileHeight);

        _tileSize =
            Mathf.Clamp(
                calculatedTileSize,
                minTileSize,
                maxTileSize);

        float boardWidth =
            width * _tileSize;

        float boardHeight =
            height * _tileSize;

        _origin =
            new Vector2(
                -boardWidth * 0.5f +
                _tileSize * 0.5f,

                -boardHeight * 0.5f +
                _tileSize * 0.5f);
                
        _origin += boardOffset;
    }

    public Vector3 GetWorldPosition(TilePosition position)
    {
        return new Vector3(
            _origin.x +
            position.X * _tileSize,

            _origin.y +
            position.Y * _tileSize,

            0f);
    }

    public Vector3 GetSpawnWorldPosition(int column, IReadOnlyPuzzleBoard board)
    {
        int highestY = 0;

        for (int y = board.Height - 1; y >= 0; y--)
        {
            TilePosition pos = new TilePosition(column, y);

            if (board.IsInside(pos) == false)
            {
                continue;
            }

            highestY = y;

            break;
        }

        Vector3 highest =  GetWorldPosition(new TilePosition(column, highestY));

        return highest + Vector3.up * TileSize;
    }

    public Vector2 GetBoardSize()
    {
        return new Vector2( _width * _tileSize, _height * _tileSize);
    }

    public Rect GetBoardBounds()
    {
        Vector2 size = GetBoardSize();

        return new Rect(
            _origin.x - _tileSize * 0.5f,
            _origin.y - _tileSize * 0.5f,
            size.x,
            size.y);
    }

    public bool TryGetTilePosition(
        Vector3 worldPosition,
        out TilePosition position)
    {
        float minX =
            _origin.x - _tileSize * 0.5f;

        float minY =
            _origin.y - _tileSize * 0.5f;

        int x =
            Mathf.FloorToInt(
                (worldPosition.x - minX) /
                _tileSize);

        int y =
            Mathf.FloorToInt(
                (worldPosition.y - minY) /
                _tileSize);

        if (x < 0 ||
            x >= _width ||
            y < 0 ||
            y >= _height)
        {
            position = default;

            return false;
        }

        position =
            new TilePosition(x, y);

        return true;
    }
}