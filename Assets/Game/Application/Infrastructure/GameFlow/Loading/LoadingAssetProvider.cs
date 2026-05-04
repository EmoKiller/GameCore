using System.Collections.Generic;
using UnityEngine;

public interface ILoadingAssetProvider
{
    IReadOnlyList<string> GetAssetLabels();
}
public sealed class MainMenuAssetProvider : ILoadingAssetProvider
{
    public IReadOnlyList<string> GetAssetLabels()
        => new[]
        {
            "mainmenu"
        };
}
public sealed class GamePlayAssetProvider : ILoadingAssetProvider
{
    public IReadOnlyList<string> GetAssetLabels()
        => new[]
        {
            "mainmenu"
        };
}