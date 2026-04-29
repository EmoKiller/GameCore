public readonly struct CharacterAnimationRequest
{
    public ECharacterAnimationState State { get; }

    // Chọn biến thể animation (combo, attack type…)
    public int Variant { get; }

    // Dùng cho BlendTree (Locomotion)
    public float SpeedParam { get; }

    // Tốc độ playback của animation
    public float PlaybackSpeed { get; }

    // Có force restart state hay không
    public bool ForceRestart { get; }

    public CharacterAnimationRequest(
        ECharacterAnimationState state,
        int variant = 0,
        float speedParam = 0f,
        float playbackSpeed = 1f,
        bool forceRestart = false)
    {
        State = state;
        Variant = variant;
        SpeedParam = speedParam;
        PlaybackSpeed = playbackSpeed;
        ForceRestart = forceRestart;
    }
}