
using UnityEngine;
public interface ICharacterView
{
    CharacterContext CharacterContext {get;}
    Transform GetTransform();
    GameObject GetGameObject();
}

public class CharacterView: MonoBehaviour , ICharacterView
{
    public CharacterContext CharacterContext{ get; private set; }

    void Awake()
    {
        var movement = GetComponent<ICharacterMovement>();
        var animator = GetComponent<ICharacterAnimator>();
        var sensor = GetComponent<ICharacterSensor>();

        if (movement == null)
        {
            Debug.LogError($"ICharacterMovement missing on {gameObject.name}");
            return;
        }
        if (animator == null)
        {
            Debug.LogError($"ICharacterAnimator missing on {gameObject.name}");
            return;
        }
        if (sensor == null)
        {
            Debug.LogError($"ICharacterSensor missing on {gameObject.name}");
            return;
        }
        CharacterContext = new CharacterContext(movement, animator, sensor);
    }

    public Transform GetTransform()
    {
        return transform;
    }
    public GameObject GetGameObject()
    {
        return gameObject;
    }
}
