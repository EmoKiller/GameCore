using System.Collections.Generic;
using UnityEngine;
public interface ISpecialActivationChainProcessor
{
    void Process(
        PuzzleBoard board,
        IEnumerable<TilePosition> positions,
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
        IEnumerable<TilePosition> positions,
        BoardChangeSet changeSet)
    {
        Queue<TilePosition> queue = new Queue<TilePosition>();

        HashSet<TilePosition> visited = new HashSet<TilePosition>();

        foreach (TilePosition pos in positions)
        {
            queue.Enqueue(pos);
        }

        while (queue.Count > 0)
        {
            TilePosition current = queue.Dequeue();

            if (visited.Contains(current))
            {
                continue;
            }

            visited.Add(current);

            TileData tile = board.Get(current);

            if (tile.HasSpecial == false)
            {
                continue;
            }

            _activationProcessor.Activate(
                board,
                current,
                changeSet);
        }
    }
}
