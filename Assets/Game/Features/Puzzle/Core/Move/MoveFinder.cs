
using System.Collections.Generic;

public interface IMoveFinder
{
    IReadOnlyList<SwapMove> FindAllMoves(IGrid grid);
}
public sealed class MoveFinder : IMoveFinder
{
    private readonly ISwapValidator _validator;

    public MoveFinder(ISwapValidator validator)
    {
        _validator = validator;
    }

    public IReadOnlyList<SwapMove> FindAllMoves(IGrid grid)
    {
        var moves = new List<SwapMove>();

        for (int x = 0; x < grid.Width; x++)
        {
            for (int y = 0; y < grid.Height; y++)
            {
                // check right
                if (x + 1 < grid.Width &&
                    _validator.CanSwap(grid, x, y, x + 1, y))
                {
                    moves.Add(new SwapMove(x, y, x + 1, y));
                }

                // check up
                if (y + 1 < grid.Height &&
                    _validator.CanSwap(grid, x, y, x, y + 1))
                {
                    moves.Add(new SwapMove(x, y, x, y + 1));
                }
            }
        }

        return moves;
    }
}