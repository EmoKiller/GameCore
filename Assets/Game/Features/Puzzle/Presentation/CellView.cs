using UnityEngine;

public class CellView : MonoBehaviour
{
     [SerializeField] private SpriteRenderer _background;

    private TileView _tile;

    public void SetTile(TileView tile)
    {
        _tile = tile;
    }

    public TileView GetTile() => _tile;
}
