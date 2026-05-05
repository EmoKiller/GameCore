public static class BoardPresetBuilder
{
    public static PuzzleBoard Build(BoardPreset preset)
    {
        PuzzleBoard board = new PuzzleBoard(
                preset.Width,
                preset.Height);

        foreach (TilePresetData tile in preset.Tiles)
        {
            TilePosition position =
                new TilePosition(
                    tile.Position.x,
                    tile.Position.y);

            TileData tileData =
                new TileData(
                    tile.TileType,
                    tile.Special);

            board.Set(position, tileData);
        }

        return board;
    }
}