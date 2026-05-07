using System.Collections.Generic;
using UnityEngine;
public interface ISpecialActivationChainProcessor
{
    void  Process(
        PuzzleBoard board,
        IEnumerable<SpecialActivationRequest> activations,
        BoardChangeSet changeSet);
}
public sealed class SpecialActivationChainProcessor : ISpecialActivationChainProcessor
{
    private readonly ISpecialActivationProcessor _activationProcessor;

    public SpecialActivationChainProcessor(ISpecialActivationProcessor activationProcessor)
    {
        _activationProcessor = activationProcessor;
    }

    public void Process(
        PuzzleBoard board,
        IEnumerable<SpecialActivationRequest> activations,
        BoardChangeSet changeSet)
    {
        Queue<SpecialActivationRequest> queue = new();

        HashSet<TilePosition> visited = new();

        foreach (var activation in activations)
        {
            queue.Enqueue(activation);
        }

        while (queue.Count > 0)
        {
            SpecialActivationRequest current = queue.Dequeue();

            if (visited.Contains(current.Position))
            {
                continue;
            }

            visited.Add(current.Position);

            SpecialActivationResult result =
                _activationProcessor.Activate(
                    board,
                    current,
                    changeSet);

            foreach (TilePosition next in result.TriggeredSpecials)
            {
                TileData tile = board.Get(next);

                if (tile.HasSpecial == false)
                {
                    continue;
                }

                queue.Enqueue(new SpecialActivationRequest(
                        next,
                        tile));
            }
        }

    }
}
