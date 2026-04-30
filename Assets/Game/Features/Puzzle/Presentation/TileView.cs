using System;
using UnityEngine;

public sealed class TileView : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private TileSpriteDatabase _database;

    public event Action<TileView> Clicked;
     public Vector2Int GridPosition { get; private set; }
    private void Awake()
    {
        if (_renderer == null)
        {
            _renderer = GetComponent<SpriteRenderer>();
            return;
        }
        if(_renderer == null)
        {
            Debug.LogError("SpriteRenderer == null in object " + gameObject.name);
        }
        
    }
    public void SetType(ETileType type)
    {
        var sprite = _database.Get(type);
        _renderer.sprite = sprite;
        //_renderer.color = Color.white; 
    }
    public void SetEmpty()
    {
        _renderer.sprite = null;
    }
    public void SetPosition(Vector3 pos)
    {
        transform.localPosition = pos;
    }
    private void OnMouseDown()
    {
        Clicked?.Invoke(this);
    }
    public void SetGridPosition(int x, int y)
    {
        GridPosition = new Vector2Int(x, y);
    }
}