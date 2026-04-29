using UnityEngine;

/// <summary>
/// Base movement component for Character, handling basic movement logic.
/// Thành phần chuyển động cơ bản cho Nhân vật, xử lý logic chuyển động cơ bản.
/// 
/// This component is designed to be simple and reusable across different Character types (Player, Enemy, NPC).
/// Nó được thiết kế để đơn giản và có thể tái sử dụng cho các loại Nhân vật khác nhau (Người chơi, Kẻ thù, NPC).
/// 
/// It is responsible for processing movement input and applying it to the character's position.
/// Nó chịu trách nhiệm xử lý input di chuyển và áp dụng nó vào vị trí của nhân vật.
/// </summary>


public abstract class CharacterMovementComponent : ICharacterMovement
{

    // ==== Config ====
    [Header("Movement Settings")]
    /// <summary>
    /// Acceleration / Deceleration - how quickly the character reaches max speed or stops.
    // Gia tốc / Phanh - tốc độ mà nhân vật đạt đến tốc độ tối đa hoặc dừng lại.
    /// </summary>
    [SerializeField] protected float acceleration = 40f;
    [SerializeField] protected float deceleration = 25f;
    [SerializeField] protected float maxWalkSpeed = 3f;
    [SerializeField] protected float maxRunSpeed = 6f;

    protected float CurrentMaxSpeed => isSprinting ? maxRunSpeed  : maxWalkSpeed;
    protected bool isSprinting;


    [Header("Jump Settings")]
    protected float jumpForce = 12f;

    

    // ===== RUNTIME =====
    public abstract Vector3 ActualVelocity { get; }

    public float CurrentSpeed => ActualVelocity.magnitude;

    

    public float TargetVelocityX
    {
        get
        {
            if (Mathf.Abs(moveInput.x) < 0.01f)
                return 0f;
            
            float targetSpeedX = Mathf.Abs(TargetVelocity.x);
            return Mathf.Clamp01(targetSpeedX / maxRunSpeed);
        }
    }

    /// <summary>Input di chuyển (x,z cho 3D | x cho 2D)</summary>
    protected Vector3 moveInput;


    // ===== INTERNAL STATE =====
    protected Vector3 TargetVelocity => moveInput * CurrentMaxSpeed;

    protected float CurrentAcceleration => moveInput.sqrMagnitude > 0.01f ? acceleration : deceleration;


    // ===== CONFIG API =====
    
   
    
    // ===== CONTROL API =====
    
    public void SetMoveInput(float input)
    {
        moveInput = new Vector3(Mathf.Clamp(input, -1f, 1f), 0f, 0f);
    }


    /// <summary>Di chuyển theo hướng input</summary>
    public void Move(Vector3 direction)
    {
        moveInput = new Vector3(direction.x, 0f, direction.y);
    }
    
    public void ResetMovement()
    {
        moveInput = Vector3.zero;
    }
    
    

    public virtual void Jump()
    {
        // Implement jump logic in derived classes if needed
    }

    public void SetSprint(bool value)
    {
        isSprinting = value;
    }
    /// <summary>Reset input khi không di chuyển</summary>
    
    

    // ===== CORE =====
    /// <summary>
    /// Apply movement based on current input and speed.
    /// Áp dụng chuyển động dựa trên input hiện tại và tốc độ.
    /// </summary>
    /// <param name="deltaTime"></param>
    protected abstract  void ApplyMovement(float deltaTime);

    
}
