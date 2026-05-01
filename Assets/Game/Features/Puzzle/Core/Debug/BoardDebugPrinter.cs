using System.Text;
using UnityEngine;

public static class BoardDebugPrinter
{
    public static void Print(PuzzleBoard board)
    {
        var builder = new StringBuilder();

        for (int y = board.Height - 1; y >= 0; y--)
        {
            for (int x = 0; x < board.Width; x++)
            {
                var tile = board.Get(x, y);

                builder.Append(GetSymbol(tile.Type));
                builder.Append(" ");
            }

            builder.AppendLine();
        }

        Debug.Log(builder.ToString());
    }
    public static void Print(MatchResult result)
    {
        foreach (var group in result.Groups)
        {
            Debug.Log(
                $"{group.TileType} Match: " +
                $"{string.Join(",", group.Positions)}");
        }
    }
    public static void Print(CascadeResult result)
    {
        Debug.Log(
            $"Cascade Count: {result.ChainCount}");

        for (int i = 0; i < result.Steps.Count; i++)
        {
            var step = result.Steps[i];

            Debug.Log(
                $"Step {i + 1} Matches: " +
                $"{step.MatchResult.Groups.Count}");
        }
    }
    private static char GetSymbol(ETileType type)
    {
        return type switch
        {
            ETileType.Sword => 'S',
            ETileType.Heart => 'H',
            ETileType.Shield => 'D',
            ETileType.Coin => 'C',
            ETileType.None => 'N',
            _ => '.'
        };
    }
}