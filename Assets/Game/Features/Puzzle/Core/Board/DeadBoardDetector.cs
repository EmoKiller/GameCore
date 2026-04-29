
public interface IDeadBoardDetector
{
    bool IsDead(IGrid grid);
}
public sealed class DeadBoardDetector : IDeadBoardDetector
{
    private readonly IMoveFinder _moveFinder;

    public DeadBoardDetector(IMoveFinder moveFinder)
    {
        _moveFinder = moveFinder;
    }

    public bool IsDead(IGrid grid)
    {
        return _moveFinder.FindAllMoves(grid).Count == 0;
    }
}
