using UnityEngine;
public interface ISpecialActivationProcessor
{
    SpecialActivationResult Activate(PuzzleBoard board, SpecialActivationRequest request, BoardChangeSet changeSet);
}
public sealed class SpecialActivationProcessor : ISpecialActivationProcessor
{
    public SpecialActivationResult Activate(
        PuzzleBoard board,
        SpecialActivationRequest request,
        BoardChangeSet changeSet)
    {
        TileData tile = request.Tile;

        TilePosition position = request.Position;

        TileSpecialData special = tile.Special;

        if (special == null)
        {
            return SpecialActivationResult.Empty();
        }

        SpecialTileBehaviour behaviour = special.Behaviour;

        SpecialActivationResult result = behaviour.Activate(board, tile, position, changeSet);

        if (result.ConsumePolicy == ESpecialConsumePolicy.Destroy)
        {
            changeSet.Add(new RemoveTransition(position));
            board.Clear(position);
        }
        else
        {
            changeSet.Protect(position);
        }
        return result;
    }
}
