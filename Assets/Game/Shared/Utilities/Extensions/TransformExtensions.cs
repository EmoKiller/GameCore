using UnityEngine;

public static class TransformExtensions
{
    public static void ResetLocal(this Transform t)
    {
        t.localPosition = Vector3.zero;
        t.localRotation = Quaternion.identity;
    }
}
