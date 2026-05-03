using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class PuzzleBoardViewFactory 
{
    private IAssetProvider _assetProvider;
    public PuzzleBoardViewFactory(IAssetProvider assetProvider)
    {
        _assetProvider = assetProvider;
    }
    public async UniTask<PuzzleBoardView> Create(
        IPuzzleService puzzleService,
        IReadOnlyBoardLayout boardLayout,
        CancellationToken ct)
    {
        var handlePuzzleBoardView = await _assetProvider.LoadAsync<GameObject>("PuzzleBoardView", ct);
        var handleCellView = await _assetProvider.LoadAsync<GameObject>("BoardCellView", ct);
        var handleTileView = await _assetProvider.LoadAsync<GameObject>("TileView", ct);
        var handleTileVisualDatabase = await _assetProvider.LoadAsync<ScriptableObject>("TileVisualDatabase", ct);

        var cellView = handleCellView.Asset.GetComponent<BoardCellView>();
        var tileView = handleTileView.Asset.GetComponent<TileView>();
        var tileVisualDatabase = handleTileVisualDatabase.Asset as TileVisualDatabase;
        var puzzleBoardView = handlePuzzleBoardView.Asset.GetComponent<PuzzleBoardView>();
        
        var instance = Object.Instantiate(puzzleBoardView);


        instance.InitializeBoard(puzzleService, boardLayout, cellView, tileView, tileVisualDatabase);
        
        return instance;
    }
    
}
