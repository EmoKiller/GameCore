using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine;

public sealed class BoardAnimationController
{
    public async UniTask SwapAsync(
        TileView a,
        TileView b,
        float duration)
    {
        var posA = a.transform.localPosition;
        var posB = b.transform.localPosition;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / duration);

            a.transform.localPosition = Vector3.Lerp(posA, posB, t);
            b.transform.localPosition = Vector3.Lerp(posB, posA, t);

            await UniTask.Yield();
        }

        a.transform.localPosition = posB;
        b.transform.localPosition = posA;
    }
}