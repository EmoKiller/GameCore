
using UnityEngine;
public interface ICharacterView
{
    Transform GetTransform();
    GameObject GetGameObject();
}

public class CharacterView: MonoBehaviour , ICharacterView
{
    /// <summary>
    /// đang bị leak unity vào core
    /// </summary>
    /// <returns></returns>
    public Transform GetTransform()
    {
        return transform;
    }
    public GameObject GetGameObject()
    {
        return gameObject;
    }
}
