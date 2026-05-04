using UnityEngine;
public interface ISpecialActivationProcessor
{
    SpecialActivationResult Activate(PuzzleBoard board, TilePosition position, BoardChangeSet changeSet);
}
public sealed class SpecialActivationProcessor : ISpecialActivationProcessor
{
    public SpecialActivationResult Activate(
        PuzzleBoard board,
        TilePosition position,
        BoardChangeSet changeSet)
    {
        TileData tile = board.Get(position);

        if (tile.HasSpecial == false)
        {
            return SpecialActivationResult.Empty();
        }
        
        SpecialTileBehaviour behaviour =
            tile.Special.Behaviour;

        
        changeSet.Add(new RemoveTransition(position));
        board.Clear(position);

        return behaviour.Activate(
            board,
            position,
            changeSet);
    }
}
