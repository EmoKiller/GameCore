
using System.Collections.Generic;

public interface IMoveFinder
{
    bool HasAnyMove(IReadOnlyGrid grid);
}
public sealed class MoveFinder : IMoveFinder
{
    private readonly ISwapSimulator _simulator;

    public MoveFinder(ISwapSimulator simulator)
    {
        _simulator = simulator;
    }

    public bool HasAnyMove(IReadOnlyGrid grid)
    {
        for (int x = 0; x < grid.Width; x++)
        {
            for (int y = 0; y < grid.Height; y++)
            {
                // check right
                if (x + 1 < grid.Width &&
                    _simulator.WouldCreateMatch(grid, x, y, x + 1, y))
                    return true;

                // check up
                if (y + 1 < grid.Height &&
                    _simulator.WouldCreateMatch(grid, x, y, x, y + 1))
                    return true;
            }
        }

        return false;
    }
}