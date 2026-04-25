using UnityEngine;

public interface IcameraView
{
    Transform Transform { get; }
    void Destroy();
}

public class CameraView : MonoBehaviour, IcameraView
{
    public Transform Transform => transform;
    public void Destroy()
    {
        if (gameObject != null) Object.Destroy(gameObject);
    }

}
