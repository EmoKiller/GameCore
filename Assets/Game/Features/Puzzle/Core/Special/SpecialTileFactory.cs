using UnityEngine;

public class SpecialTileFactory 
{
    public void CreateSpecial(
        PuzzleBoard board,
        SpecialSpawnResult result,
        ETileType baseType
    )
    {
        TileData tile = new TileData(baseType, result.SpecialType);

        board.Set(result.SpawnPosition, tile);

        Debug.Log(tile.SpecialType);
    }
}
