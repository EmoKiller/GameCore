using UnityEngine;
public interface ISpecialActivationProcessor
{
    void Activate(PuzzleBoard board, TilePosition position, BoardChangeSet changeSet);
}
public sealed class SpecialActivationProcessor : ISpecialActivationProcessor
{
    public void Activate(PuzzleBoard board, TilePosition position, BoardChangeSet changeSet)
    {
        TileData tile = board.Get(position);

        if (tile.HasSpecial == false)
        {
            return;
        }

        tile.Special.Behaviour.Activate(
            board,
            position,
            changeSet);
    }
}
