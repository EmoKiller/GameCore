using System.Collections.Generic;
using System.Linq;

public sealed class PuzzleSessionService
{
    private readonly PuzzleSessionState _state;

    private readonly List<IPuzzleGoal> _goals;

    public PuzzleSessionService(
        PuzzleSessionState state,
        List<IPuzzleGoal> goals)
    {
        _state = state;
        _goals = goals;
    }

    public void ProcessMove(CascadeResult cascadeResult)
    {
        _state.ConsumeMove();

        ProcessScore(cascadeResult);

        ProcessGoals(cascadeResult);

        EvaluateGameState();
    }
    private void ProcessScore(
        CascadeResult cascadeResult)
    {
        int score = 0;

        // foreach (CascadeStepResult step in cascadeResult.Steps)
        // {
        //     foreach (TileMatch match in step.MatchResult.Matches)
        //     {
        //         score += match.Positions.Count * 10;
        //     }
        // }

        _state.AddScore(score);
    }
    private void ProcessGoals(
        CascadeResult cascadeResult)
    {
        foreach (CascadeStepResult step in cascadeResult.Steps)
        {
            foreach (IPuzzleGoal goal in _goals)
            {
                goal.Process(step.MatchResult);
            }
        }
    }
    private void EvaluateGameState()
    {
        bool allCompleted = _goals.All(x => x.IsCompleted);

        if (allCompleted)
        {
            _state.MarkWin();

            return;
        }

        if (_state.RemainingMoves <= 0)
        {
            _state.MarkLose();
        }
    }
}