

public sealed class CascadeStepResult
{
    public MatchResult MatchResult { get; }

    public BoardChangeSet ChangeSet { get; }

    public CascadeStepResult(
        MatchResult matchResult,
        BoardChangeSet changeSet)
    {
        MatchResult = matchResult;
        ChangeSet = changeSet;
    }
    
}
