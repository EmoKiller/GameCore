using System;
using UnityEngine;

public class BoardCellView : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer _iconRenderer;
    public void SetSprite(Sprite sprite)
    {
        _iconRenderer.sprite = sprite;
    }
    public void SetSize(float size)
    {
        transform.localScale =
            Vector3.one * size;
    }

    public void SetWorldPosition(Vector3 position)
    {
        transform.position = position;
    }
}
