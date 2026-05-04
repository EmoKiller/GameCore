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
    public async UniTask MoveToAsync(Vector3 target, float duration)
    {
        Vector3 start =
            transform.position;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float progress = Mathf.Clamp01(
                    elapsed / duration);

            // Ease In (gravity feel)
            // float eased = 1f - (1f - progress) * (1f - progress);
            float eased = progress;

            transform.position =
                Vector3.Lerp(
                    start,
                    target,
                    eased);

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
    public void SetSpecial( TileData tile, SpecialTileVisualDatabase database)
    {
        Debug.Log(tile.SpecialType);

        if (tile.SpecialType == ETileSpecialType.None)
        {
            _specialRenderer.enabled = false;
            _specialRenderer.sprite = null;

            return;
        }

        Sprite sprite = database.GetSprite(tile.Type, tile.SpecialType);

        _specialRenderer.enabled = true;

        _specialRenderer.sprite = sprite;
    }
}
