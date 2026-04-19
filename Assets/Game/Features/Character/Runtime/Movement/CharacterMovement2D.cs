using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
public sealed class CharacterMovement2D : CharacterMovementComponent
{
    private Rigidbody2D rb;
    
    public override Vector3 ActualVelocity => rb.linearVelocity;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    private void FixedUpdate()
    {
        ApplyMovement(Time.fixedDeltaTime);
    }

    protected override void ApplyMovement(float deltaTime)
    {
        // Tính velocity X mới với acceleration / deceleration
        float newVelocityX = Mathf.MoveTowards(
            rb.linearVelocity.x,
            TargetVelocity.x,
            CurrentAcceleration * deltaTime
        );

        // Giữ nguyên velocity Y (gravity / jump)
        rb.linearVelocity = new Vector2(newVelocityX, rb.linearVelocity.y);
    }

    public override void Jump()
    {   
        // Reset vận tốc Y trước khi nhảy để lực nhảy luôn đồng nhất
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f); 
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        
        // (Tùy chọn) Ngắt Coyote Time ngay sau khi nhảy để tránh nhảy 2 lần trong không trung
        // sensor.ResetCoyoteTime(); // Nếu bạn muốn viết thêm hàm này bên Sensor
    }
}
