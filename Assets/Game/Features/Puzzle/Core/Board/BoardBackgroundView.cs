using UnityEngine;

public class BoardBackgroundView : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer _renderer;

    public void SetSprite(Sprite sprite)
    {
        _renderer.sprite = sprite;
    }

    public void SetSize(float width, float height)
    {
        transform.localScale = new Vector3(width, height, 1f);
    }
}
