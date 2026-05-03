using UnityEngine;

public class SpecialTileFactory 
{
    public TileData CreateSpecial(
        PuzzleBoard board,
        TilePosition position,
        ETileSpecialType specialType)
    {
        TileData baseTile = board.Get(position);

        TileData specialTile = new TileData(
                baseTile.Type,
                specialType);

        board.Set(position, specialTile);

        return specialTile;
    }
}
