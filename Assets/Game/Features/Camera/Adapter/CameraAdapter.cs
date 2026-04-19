using UnityEngine;

public class CameraAdapter : MonoBehaviour
{
    private CameraController _controller;

    public void Initialize(CameraController controller)
    {
        _controller = controller;
    }
    private void LateUpdate()
    {
        _controller?.Update(Time.deltaTime);
    }
}
