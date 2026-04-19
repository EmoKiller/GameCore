using UnityEngine.SceneManagement;
using System.Threading;
using Cysharp.Threading.Tasks;
using System;
namespace Game.Application.Core.SceneLoader
{
    public interface ISceneLoader : IService
    {
        UniTask LoadScene( string sceneName, IProgress<float> progress, CancellationToken ct);
    }

    public class SceneLoader : ISceneLoader
    {
        public async UniTask LoadScene( string sceneName, IProgress<float> progress, CancellationToken ct)
        {
            var op = SceneManager.LoadSceneAsync(sceneName);
            op.allowSceneActivation = false;

            while (op.progress < 0.9f)
            {
                ct.ThrowIfCancellationRequested();

                // Unity progress: 0 → 0.9
                progress?.Report(op.progress);

                await UniTask.Yield();
            }

            // map 0.9 → 1.0
            progress?.Report(1f);

            op.allowSceneActivation = true;

            while (!op.isDone)
            {
                ct.ThrowIfCancellationRequested();
                await UniTask.Yield();
            }
        }
    }
}

