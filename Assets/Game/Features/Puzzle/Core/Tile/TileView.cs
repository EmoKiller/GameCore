using Cysharp.Threading.Tasks;
using UnityEngine;

public sealed class TileView : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer _spriteRenderer;

    [SerializeField]
    private SpriteRenderer _specialRenderer;
    private void Awake() 
    {
        _specialRenderer.enabled = false;
    }

    public TilePosition Position { get; private set; }

    public void SetPosition(TilePosition position)
    {
        Position = position;
    }
    public void SetInstantPosition(
        Vector3 position)
    {
        transform.position = position;
    }
    public void SetSprite(Sprite sprite)
    {
        _spriteRenderer.sprite = sprite;
    }

    public void SetWorldPosition(Vector3 position)
    {
        transform.position = position;
    }
    public async UniTask MoveToAsync(
        Vector3 target,
        float duration)
    {
        Vector3 start = transform.position;

        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;

            float t = time / duration;
            
            t = 1f - Mathf.Pow(1f - t, 3f);
            
            transform.position =
                Vector3.Lerp(
                    start,
                    target,
                    t);

            await UniTask.Yield();
        }

        transform.position = target;
    }
    public async UniTask ScaleToAsync( Vector3 target, float duration)
    {
        Vector3 start =
            transform.localScale;

        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;

            float t = time / duration;
            t = 1f - Mathf.Pow(1f - t, 3f);
            

            transform.localScale =
                Vector3.Lerp(
                    start,
                    target,
                    t);

            await UniTask.Yield();
        }

        transform.localScale = target;
    }
    public void ResetVisual()
    {
        _specialRenderer.enabled = false;

        _specialRenderer.sprite = null;

        transform.localScale =Vector3.one;
        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
    }
    public void SetSortingOrder(int order)
    {
        _spriteRenderer.sortingOrder = order;
    }
    public void SetSpecial(ETileSpecialType type, SpecialTileVisualDatabase database)
    {
        Debug.Log(type);
        if (type == ETileSpecialType.None)
        {
            _specialRenderer.enabled = false;

            return;
        }

        _specialRenderer.enabled = true;

        switch (type)
        {
            case ETileSpecialType.HorizontalRocket:

                _specialRenderer.sprite =
                    database.HorizontalRocket;

                break;

            case ETileSpecialType.VerticalRocket:

                _specialRenderer.sprite =
                    database.VerticalRocket;

                break;
        }
    }
}
