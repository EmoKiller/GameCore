using System;
using UnityEngine;


public enum AnimationEventType
{
    /// <summary>
    /// enable hitbox
    /// </summary>
    AttackStart = 0, 

    /// <summary>
    /// apply damage
    /// </summary>
    AttackHit = 1,
     
    /// <summary>
    /// disable hitbox  
    /// </summary>
    AttackEnd = 2,   
}
public class AnimationController : MonoBehaviour , ICharacterAnimator
{
    
    private Animator animator;

    private static readonly int MoveSpeed = Animator.StringToHash("MoveSpeed");
    private static readonly int SpeedVertical = Animator.StringToHash("SpeedVertical");
    private static readonly int IsGrounded = Animator.StringToHash("IsGrounded");
    private static readonly int AttackIndex = Animator.StringToHash("AttackIndex");
    private static readonly int Attack = Animator.StringToHash("Attack");

    private AnimatorStateInfo State => animator.GetCurrentAnimatorStateInfo(BaseLayer);

    private const int BaseLayer = 0;

    public event Action<AnimationEventType> OnAnimationEvent;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator not found on " + gameObject.name);
        }
    }

    /// <summary>
    /// Hàm này sẽ thiết lập tốc độ di chuyển trong Animator,
    /// cho phép chuyển đổi giữa các animation đi bộ, chạy và đứng yên.
    /// </summary>
    /// <param name="speed"></param>
    public void SetMoveSpeed(float speed)
    {
        animator.SetFloat(MoveSpeed, speed ,0.1f, Time.deltaTime);
    }

    /// <summary>
    /// Hàm này sẽ thiết lập tốc độ di chuyển trong Animator,
    /// cho phép chuyển đổi giữa các animation đi bộ, chạy và đứng yên.
    /// </summary>
    /// <param name="speed"></param>
    public void SetSpeedVertical(float speed)
    {
        animator.SetFloat(SpeedVertical, speed);
    }

    /// <summary>
    /// Hàm này sẽ thiết lập trạng thái grounded trong Animator,
    /// cho phép chuyển đổi giữa các animation nhảy và đi bộ/dừng lại.
    /// </summary>
    /// <param name="grounded"></param>
    public void SetGrounded(bool grounded)
    {
        animator.SetBool(IsGrounded, grounded);
    }

    /// <summary>
    /// Hàm này sẽ thiết lập chỉ số combo tấn công hiện tại trong Animator.
    /// </summary>
    /// <param name="combo"></param>
    public void SetCombo(int combo)
    {
        animator.SetInteger(AttackIndex, combo);
    }

    /// <summary>
    /// Hàm này sẽ kích hoạt trigger Attack, bắt đầu một animation tấn công mới dựa trên giá trị hiện tại của AttackIndex.
    /// </summary>
    public void TriggerAttack()
    {
        animator.SetTrigger(Attack);
    }

    /// <summary>
    /// Hàm này sẽ reset trigger Attack, đảm bảo rằng nếu có một combo attack đang diễn ra, nó sẽ bị hủy bỏ và không thể tiếp tục.
    /// </summary>
    public void ResetAttack()
    {
        animator.ResetTrigger(Attack);
    }

    /// <summary>
    /// Kiểm tra xem animation hiện tại có phải là stateName hay không.
    /// </summary>
    /// <param name="stateName"></param>
    /// <returns></returns>
    public bool IsPlaying(string stateName)
    {
        return State.IsName(stateName);
    }

    /// <summary>
    /// Lấy thời gian đã trôi qua của animation hiện tại, được chuẩn hóa từ 0 đến 1.
    /// Khi animation lặp lại, giá trị này sẽ tiếp tục tăng lên 
    /// (ví dụ: 1.5 có nghĩa là animation đã hoàn thành một lần và đang ở giữa lần thứ hai).
    /// </summary>
    public float GetNormalizedTime()
    {
        return State.normalizedTime;
    }

    /// <summary>
    /// Kiểm tra xem animation đã trôi qua một khoảng thời gian chuẩn hóa nhất định hay chưa.
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public bool IsPastNormalizedTime(float time)
    {
        return GetNormalizedTime() >= time;
    }

    /// <summary>
    /// Kiểm tra xem animation đã hoàn thành hay chưa (đã trôi qua normalizedTime >= 1).
    /// </summary>
    /// <returns></returns>
    public bool IsAnimationFinished()
    {
        return GetNormalizedTime() >= 1f;
    }

    /// <summary>
    /// Kiểm tra xem animation hiện tại đã trôi qua một khoảng thời gian chuẩn hóa nhất định nhưng chưa hoàn thành hay chưa.
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    public bool IsInNormalizedWindow(float start, float end)
    {
        float time = GetNormalizedTime();
        return time >= start && time <= end;
    }

    /// <summary>
    /// Kiểm tra xem animator có đang trong quá trình chuyển đổi giữa hai state hay không.
    /// </summary>
    /// <returns></returns>
    public bool IsInTransition()
    {
        return animator.IsInTransition(BaseLayer);
    }

    /// <summary>
    /// Hàm này sẽ được gọi từ các sự kiện animation trong Animator.
    /// Nó sẽ chuyển đổi eventId thành AnimationEventType và kích hoạt sự kiện OnAnimationEvent.
    /// </summary>
    /// <param name="eventId"></param>
    public void AnimationEvent(int eventId)
    {
        if (eventId >= 0 && eventId <= (int)AnimationEventType.AttackEnd)
        {
            OnAnimationEvent?.Invoke((AnimationEventType)eventId);
        }
        else
        {
            Debug.LogWarning($"Unknown AnimationEventType: {eventId}");
        }
    }
}
