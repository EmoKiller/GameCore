using UnityEngine;

public sealed class PuzzleSwapTest : MonoBehaviour
{
    // private void Awake()
    // {
    //     var board = new PuzzleBoard(5, 5);

    //     SetupBoard(board);

    //     Debug.Log("Before Swap");
    //     BoardDebugPrinter.Print(board);

    //     var resolver = new MatchResolver();

    //     var swapProcessor =
    //         new SwapProcessor(resolver);

    //     bool success = swapProcessor.TrySwap(
    //         board,
    //         new TilePosition(1, 0),
    //         new TilePosition(1, 1));

    //     Debug.Log($"Swap Result: {success}");

    //     Debug.Log("After Swap");
    //     BoardDebugPrinter.Print(board);

    //     var matches = resolver.Resolve(board);

    //     BoardDebugPrinter.Print(matches);
    // }

    private void SetupBoard(PuzzleBoard board)
    {
        board.Set(0, 0, new TileData(ETileType.Heart));
        board.Set(1, 0, new TileData(ETileType.Sword));
        board.Set(2, 0, new TileData(ETileType.Heart));

        board.Set(0, 1, new TileData(ETileType.Coin));
        board.Set(1, 1, new TileData(ETileType.Heart));
        board.Set(2, 1, new TileData(ETileType.Shield));
    }
}