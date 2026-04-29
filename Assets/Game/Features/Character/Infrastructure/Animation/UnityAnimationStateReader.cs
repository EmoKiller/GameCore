using UnityEngine;
public interface IAnimationStateReader
{
    bool IsPlaying(string stateName);
    float GetNormalizedTime();
    bool IsInTransition();
}
public sealed class UnityAnimationStateReader : IAnimationStateReader
{
    private readonly Animator _animator;
    private const int BaseLayer = 0;

    public UnityAnimationStateReader(Animator animator)
    {
        _animator = animator;
    }

    public bool IsPlaying(string stateName)
    {
        return _animator.GetCurrentAnimatorStateInfo(BaseLayer).IsName(stateName);
    }

    public float GetNormalizedTime()
    {
        return _animator.GetCurrentAnimatorStateInfo(BaseLayer).normalizedTime;
    }

    public bool IsInTransition()
    {
        return _animator.IsInTransition(BaseLayer);
    }
}
