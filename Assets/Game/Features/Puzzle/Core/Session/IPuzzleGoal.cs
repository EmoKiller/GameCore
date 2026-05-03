
public interface IPuzzleGoal
{
    bool IsCompleted { get; }

    void Process(MatchResult matchResult);
}
