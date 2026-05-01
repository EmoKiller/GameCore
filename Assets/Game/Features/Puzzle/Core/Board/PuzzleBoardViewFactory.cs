using UnityEngine;

public class PuzzleBoardViewFactory 
{
    private PuzzleBoardView _prefab;
    public PuzzleBoardViewFactory(PuzzleBoardView prefab)
    {
        _prefab = prefab;
    }
    public  PuzzleBoardView Create()
    {
        var handle = GameObject.Instantiate(_prefab);

        return handle;
    }
}
