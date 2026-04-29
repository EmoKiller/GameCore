using System;

public interface ISwapValidator
{
    bool CanSwap(IGrid grid, int x1, int y1, int x2, int y2);
}
public sealed class SwapValidator : ISwapValidator
{
    private readonly ILocalMatchDetector _localDetector;

    public SwapValidator(ILocalMatchDetector localDetector)
    {
        _localDetector = localDetector;
    }

    public bool CanSwap(IGrid grid, int x1, int y1, int x2, int y2)
    {
        if (!IsAdjacent(x1, y1, x2, y2))
            return false;

        grid.Swap(x1, y1, x2, y2);

        bool result =
            _localDetector.HasMatchAt(grid, x1, y1) ||
            _localDetector.HasMatchAt(grid, x2, y2);

        grid.Swap(x1, y1, x2, y2);

        return result;
    }

    private bool IsAdjacent(int x1, int y1, int x2, int y2)
    {
        return Math.Abs(x1 - x2) + Math.Abs(y1 - y2) == 1;
    }
}