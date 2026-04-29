using Cysharp.Threading.Tasks;
using UnityEngine;
public interface ISpawnService
{
    // Sử dụng UniTask để hỗ trợ load async từ Addressables nếu cần sau này
    UniTask<T> SpawnAsync<T>(string address, Vector3 position, Quaternion rotation) where T : Component;
    void Despawn(GameObject obj);
}
public class SpawnService : ISpawnService
{
    public void Despawn(GameObject obj)
    {
        throw new System.NotImplementedException();
    }

    public UniTask<T> SpawnAsync<T>(string address, Vector3 position, Quaternion rotation) where T : Component
    {
        throw new System.NotImplementedException();
    }
}
