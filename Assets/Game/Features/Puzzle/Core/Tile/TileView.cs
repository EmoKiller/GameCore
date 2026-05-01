using Cysharp.Threading.Tasks;
using UnityEngine;

public sealed class TileView : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer _iconRenderer;

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
        _iconRenderer.sprite = sprite;
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

            transform.localScale =
                Vector3.Lerp(
                    start,
                    target,
                    t);

            await UniTask.Yield();
        }

        transform.localScale = target;
    }

    
}
