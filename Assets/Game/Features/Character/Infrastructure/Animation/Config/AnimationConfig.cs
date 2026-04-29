using UnityEngine;

[CreateAssetMenu(menuName = "Game/Animation/AnimationConfig")]
public class AnimationConfig : ScriptableObject
{
    public AnimationStateConfig[] States;

    public void Initialize()
    {
        if (States == null) return;

        foreach (var state in States)
        {
            state.Initialize();
        }
    }
}