using System;
using UnityEngine;



public interface ICharacterAnimator 
{
    // Commands
    void SetMoveSpeed(float speed);
    void SetSpeedVertical(float speed);
    void SetGrounded(bool grounded);
    void SetCombo(int combo);
    void TriggerAttack();
    void ResetAttack();
    
    // Events
    event Action<EAnimationEventType> OnAnimationEvent;
}
public sealed class UnityCharacterAnimator : ICharacterAnimator
{
    private readonly Animator _animator;

    private static readonly int MoveSpeed = Animator.StringToHash("MoveSpeed");
    private static readonly int SpeedVertical = Animator.StringToHash("SpeedVertical");
    private static readonly int IsGrounded = Animator.StringToHash("IsGrounded");
    private static readonly int AttackIndex = Animator.StringToHash("AttackIndex");
    private static readonly int Attack = Animator.StringToHash("Attack");

    public event Action<EAnimationEventType> OnAnimationEvent;

    public UnityCharacterAnimator(Animator animator)
    {
        if (animator == null)
            throw new ArgumentNullException(nameof(animator));

        _animator = animator;
    }
    /// <summary>
    /// Time.deltaTime fix
    /// </summary>
    /// <param name="speed"></param>
    public void SetMoveSpeed(float speed)
    {
        _animator.SetFloat(MoveSpeed, speed, 0.1f, Time.deltaTime);
    }

    public void SetSpeedVertical(float speed)
    {
        _animator.SetFloat(SpeedVertical, speed);
    }

    public void SetGrounded(bool grounded)
    {
        _animator.SetBool(IsGrounded, grounded);
    }

    public void SetCombo(int combo)
    {
        _animator.SetInteger(AttackIndex, combo);
    }

    public void TriggerAttack()
    {
        _animator.SetTrigger(Attack);
    }

    public void ResetAttack()
    {
        _animator.ResetTrigger(Attack);
    }

    // Called from Animation Event (Unity)
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

