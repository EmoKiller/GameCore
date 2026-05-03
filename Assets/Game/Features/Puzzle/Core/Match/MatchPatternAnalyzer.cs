using System.Linq;

public interface IMatchPatternAnalyzer
{
    SpecialSpawnResult Analyze(MatchGroup match,SwapContext swapContext);
}
public sealed class MatchPatternAnalyzer : IMatchPatternAnalyzer
{
    public SpecialSpawnResult Analyze(MatchGroup group,SwapContext swapContext)
    {
        if (group.Positions.Count != 4)
        {
            return SpecialSpawnResult.None();
        }

        bool horizontal = group.Positions.All(x =>
                    x.Y == group.Positions[0].Y
                );
                
        bool hasSwapContext = group.Positions.Contains(swapContext.To);
        TilePosition spawnPosition = hasSwapContext ? swapContext.To : group.Positions[0];

        if (horizontal)
        {
            return new SpecialSpawnResult(
                true,
                spawnPosition,
                ETileSpecialType.HorizontalRocket);
        }

        return new SpecialSpawnResult(
            true,
            spawnPosition,
            ETileSpecialType.VerticalRocket);
    }
}