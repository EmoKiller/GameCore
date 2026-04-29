using UnityEngine;

public sealed class TileView : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _renderer;
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
    public void SetColor(ETileType type)
    {
        

        _renderer.color = type switch
        {
            ETileType.Sword => Color.red,
            ETileType.Heart => Color.green,
            ETileType.Shield => Color.blue,
            ETileType.Coin => Color.yellow,
            _ => Color.white
        };
    }
}