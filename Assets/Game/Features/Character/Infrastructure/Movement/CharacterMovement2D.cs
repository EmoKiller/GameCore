using UnityEngine;

public sealed class CharacterMovement2D : CharacterMovementComponent
{
    private Rigidbody2D _rb;
    
    public override Vector3 ActualVelocity => _rb.linearVelocity;

    public CharacterMovement2D(Rigidbody2D rb)
    {
        if (rb == null)
        {
            Debug.Log("rb 2d == null on " + rb.gameObject.name);
        }
        _rb = rb;
        _rb.interpolation = RigidbodyInterpolation2D.Interpolate;
    }
    protected override void ApplyMovement(float deltaTime)
    {
        // Tính velocity X mới với acceleration / deceleration
        float newVelocityX = Mathf.MoveTowards(
            _rb.linearVelocity.x,
            TargetVelocity.x,
            CurrentAcceleration * deltaTime
        );

        // Giữ nguyên velocity Y (gravity / jump)
        _rb.linearVelocity = new Vector2(newVelocityX, _rb.linearVelocity.y);
    }

    public override void Jump()
    {   
        // Reset vận tốc Y trước khi nhảy để lực nhảy luôn đồng nhất
        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, 0f); 
        _rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        
        // (Tùy chọn) Ngắt Coyote Time ngay sau khi nhảy để tránh nhảy 2 lần trong không trung
        // sensor.ResetCoyoteTime(); // Nếu bạn muốn viết thêm hàm này bên Sensor
    }
}
