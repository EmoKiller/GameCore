using UnityEngine;

public sealed class PuzzleCascadeTest : MonoBehaviour
{
    private void Awake()
    {
        // var random = new UnityRandomProvider();

        // var board = new PuzzleBoard(8, 8);

        // var generator =
        //     new BoardGenerator(random);

        // generator.Fill(board);

        // ForceTestBoard(board);

        // Debug.Log("Before Cascade");
        // BoardDebugPrinter.Print(board);

        // var resolver = new MatchResolver();

        // var remover =
        //     new RemoveMatchedTilesProcessor();

        // var gravity =
        //     new GravityProcessor();

        // var spawner =
        //     new SpawnProcessor(random);

        // var cascade =
        //     new CascadeProcessor(
        //         resolver,
        //         remover,
        //         gravity,
        //         spawner);

        // var result = cascade.Process(board);

        // BoardDebugPrinter.Print(result);

        // Debug.Log("After Cascade");
        // BoardDebugPrinter.Print(board);
    }

    private void ForceTestBoard(PuzzleBoard board)
    {
        board.Set(0, 0, new TileData(ETileType.Heart));
        board.Set(1, 0, new TileData(ETileType.Heart));
        board.Set(2, 0, new TileData(ETileType.Heart));
    }
}
