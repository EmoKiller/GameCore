
using UnityEngine;

public sealed class PuzzleLoopTest : MonoBehaviour
{
    // private void Awake()
    // {
    //     var board = new PuzzleBoard(8, 8);

    //     var random = new UnityRandomProvider();

    //     var generator =
    //         new BoardGenerator(random);

    //     generator.Fill(board);

    //     ForceTestMatch(board);

    //     Debug.Log("Initial Board");
    //     BoardDebugPrinter.Print(board);

    //     var resolver = new MatchResolver();

    //     var matches = resolver.Resolve(board);

    //     BoardDebugPrinter.Print(matches);

    //     var remover =
    //         new RemoveMatchedTilesProcessor();

    //     remover.Remove(board, matches);

    //     Debug.Log("After Remove");
    //     BoardDebugPrinter.Print(board);

    //     var gravity = new GravityProcessor();

    //     gravity.Apply(board);

    //     Debug.Log("After Gravity");
    //     BoardDebugPrinter.Print(board);

    //     var spawner =
    //         new SpawnProcessor(random);

    //     spawner.FillEmpty(board);

    //     Debug.Log("After Spawn");
    //     BoardDebugPrinter.Print(board);
    // }

    private void ForceTestMatch(PuzzleBoard board)
    {
        board.Set(0, 0, new TileData(ETileType.Heart));
        board.Set(1, 0, new TileData(ETileType.Heart));
        board.Set(2, 0, new TileData(ETileType.Heart));
    }
}