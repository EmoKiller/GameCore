using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public sealed class PlayerFactory
{
    private GameObject _prefabAsset;
    public PlayerFactory(GameObject prefabAsset)
    {
        _prefabAsset = prefabAsset;
    }
    public PlayerController2D CreateAsync()
    {

        var go = Object.Instantiate(_prefabAsset);

        var controller = go.GetComponent<PlayerController2D>();

        return controller;
    }
    public PlayerController2D CreateAsync(
        Transform parent)
    {

        var go = Object.Instantiate(_prefabAsset, parent);

        var controller = go.GetComponent<PlayerController2D>();

        return controller;
    }
}
