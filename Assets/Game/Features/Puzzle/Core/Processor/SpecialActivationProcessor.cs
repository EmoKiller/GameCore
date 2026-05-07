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

        TileRuntimeSpecialState runtime = tile.RuntimeSpecialState;

        if (special == null)
        {
            return SpecialActivationResult.Empty();
        }

        SpecialTileBehaviour behaviour = special.Behaviour;

        SpecialActivationResult result = behaviour.Activate(board, tile, position, changeSet);

        if (result.ConsumePolicy == ESpecialConsumePolicy.Destroy)
        {
            runtime.LifecycleState = ESpecialLifecycleState.None;
            changeSet.MarkRemoved(position);
        }
        else 
        {
            runtime.LifecycleState = ESpecialLifecycleState.PendingRetrigger;
            changeSet.Protect(position);
        }
        return result;
    }
}
