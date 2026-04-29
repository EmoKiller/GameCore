using System;
using System.Collections.Generic;
using UnityEngine;
public interface ICharacterAnimationController
{
    void Play(in CharacterAnimationRequest request);

    event Action<EAnimationEventType> OnAnimationEvent;
}
public sealed class UnityCharacterAnimationController : ICharacterAnimationController
{
    private readonly Animator _animator;
    private readonly Dictionary<ECharacterAnimationState, AnimationStateConfig> _map;

    // Parameters
    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int VariantParamHash = Animator.StringToHash("Variant");

    private int _currentStateHash;

    public event Action<EAnimationEventType> OnAnimationEvent;

    public UnityCharacterAnimationController(Animator animator, AnimationConfig config)
    {
        _animator = animator ?? throw new ArgumentNullException(nameof(animator));

        config.Initialize();

        _map = new Dictionary<ECharacterAnimationState, AnimationStateConfig>();

        foreach (var state in config.States)
        {
            _map[state.State] = state;
        }
    }

    public void Play(in CharacterAnimationRequest request)
    {
        if (!_map.TryGetValue(request.State, out var config))
        {
            Debug.LogWarning($"No config for state {request.State}");
            return;
        }
        
        // ========================
        // 1. Update parameters (always)
        // ========================
        _animator.SetFloat(SpeedHash, request.SpeedParam, 0.1f, Time.deltaTime);
        _animator.SetInteger(VariantParamHash, request.Variant);
        _animator.speed = request.PlaybackSpeed;

        // ========================
        // 2. Resolve target state
        // ========================
        int targetHash = config.ResolveHash(request.Variant);
        if (!_animator.HasState(config.LayerIndex, targetHash))
        {
            Debug.LogError($"State not found: {targetHash}");
        }
        // ========================
        // 3. Avoid redundant transition
        // ========================
        if (!request.ForceRestart && _currentStateHash == targetHash)
            return;

        // ========================
        // 4. CrossFade
        // ========================
        _animator.CrossFade(
            targetHash,
            config.CrossFadeDuration,
            config.LayerIndex,
            request.ForceRestart ? 0f : float.NegativeInfinity
        );

        _currentStateHash = targetHash;
    }

    // ========================
    // Animation Event bridge
    // ========================
    public void AnimationEvent(int eventId)
    {
        if (eventId >= 0 && eventId <= (int)EAnimationEventType.AttackEnd)
        {
            OnAnimationEvent?.Invoke((EAnimationEventType)eventId);
        }
        else
        {
            Debug.LogWarning($"Unknown AnimationEventType: {eventId}");
        }
    }
}