using UnityEngine;

public abstract class SpecialTileBehaviour : ScriptableObject
{
    public abstract SpecialActivationResult Activate( PuzzleBoard board, TilePosition position, BoardChangeSet changeSet);
}
