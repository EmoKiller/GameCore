// using UnityEngine;

// public sealed class PuzzleTestBootstrap : MonoBehaviour
// {
//     private void Awake()
//     {
//         var board = new PuzzleBoard(8, 8);

//         var generator = new BoardGenerator(
//             new UnityRandomProvider());

//         generator.Fill(board);
//         board.Set(0, 0, new TileData(ETileType.Heart));
//         board.Set(0, 1, new TileData(ETileType.Heart));
//         board.Set(0, 2, new TileData(ETileType.Heart));
//         BoardDebugPrinter.Print(board);

//         var resolver = new MatchResolver();

//         var result = resolver.Resolve(board);

        


        
//         Debug.Log($"Match Count: {result.Groups.Count}");
//         BoardDebugPrinter.Print(result);

//     }
// }
