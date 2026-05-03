using UnityEngine;

[CreateAssetMenu(fileName = "PuzzleAnimationConfig", menuName = "Puzzle/PuzzleAnimationConfig")]
public sealed class PuzzleAnimationConfig :
    ScriptableObject
{
    [Header("Swap")]
    public float SwapDuration = 0.08f;

    public float InvalidSwapPause = 0.04f;

    [Header("Remove")]
    public float RemoveDuration = 0.15f;

    [Header("Fall")]
    public float FallDurationPerCell = 0.05f;

    public float MaxFallDuration = 0.2f;

    [Header("Spawn")]
    public float SpawnDuration = 0.2f;

    public float SpawnScaleDuration = 0.15f;
}