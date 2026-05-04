using UnityEngine;

public abstract class SpecialTileBehaviour : ScriptableObject
{
    public abstract void Activate( PuzzleBoard board, TilePosition position, BoardChangeSet changeSet);
}
