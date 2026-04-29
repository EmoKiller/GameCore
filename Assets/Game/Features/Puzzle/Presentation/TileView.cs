using UnityEngine;

public sealed class TileView : MonoBehaviour
{
    
    public void SetColor(ETileType type)
    {
        var renderer = GetComponent<SpriteRenderer>();

        renderer.color = type switch
        {
            ETileType.Sword => Color.red,
            ETileType.Heart => Color.green,
            ETileType.Shield => Color.blue,
            ETileType.Coin => Color.yellow,
            _ => Color.white
        };
    }
}