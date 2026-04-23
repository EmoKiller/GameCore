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
    public ICharacterView Create()
    {

        var go = Object.Instantiate(_prefabAsset);

        var characterView = go.GetComponent<ICharacterView>();

        return characterView;
    }
    public ICharacterView Create(Transform parent)
    {

        var go = Object.Instantiate(_prefabAsset, parent);

        var characterView = go.GetComponent<ICharacterView>();

        return characterView;
    }
}
