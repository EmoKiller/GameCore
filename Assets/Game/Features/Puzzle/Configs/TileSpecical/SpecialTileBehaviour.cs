using UnityEngine;

public abstract class SpecialTileBehaviour : ScriptableObject
{
    public abstract SpecialActivationResult Activate(PuzzleBoard board, TileData tile, TilePosition position, BoardChangeSet changeSet);
    public virtual TileRuntimeSpecialState CreateRuntimeState()
    {
        return null;
    }
}
