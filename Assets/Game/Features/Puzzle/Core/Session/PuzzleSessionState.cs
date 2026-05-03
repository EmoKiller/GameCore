public sealed class PuzzleSessionState
{
    public int RemainingMoves { get; private set; }

    public int Score { get; private set; }

    public bool IsWin { get; private set; }

    public bool IsLose { get; private set; }

    public PuzzleSessionState(int moves)
    {
        RemainingMoves = moves;
    }

    public void ConsumeMove()
    {
        RemainingMoves--;
    }

    public void AddScore(int score)
    {
        Score += score;
    }

    public void MarkWin()
    {
        IsWin = true;
    }

    public void MarkLose()
    {
        IsLose = true;
    }
}