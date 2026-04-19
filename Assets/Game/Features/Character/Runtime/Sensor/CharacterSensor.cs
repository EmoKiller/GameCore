using UnityEngine;


/// <summary>
/// A component that can be used to detect collisions and triggers for a character.
///  It can be used to detect when the character is grounded, when it is touching a wall,
///  or when it is in the air. It can also be used to detect when the character is colliding with other objects, such as enemies or pickups.
/// The CharacterSensor can be used in conjunction with a CharacterController to provide more detailed information about the character's state and interactions with the environment. It can be used to implement features such as wall jumping, ledge grabbing, or detecting when the character is in a specific area (e.g., a trigger zone).
/// The CharacterSensor can be implemented using Unity's built-in collision and trigger detection methods, such as OnCollisionEnter, OnCollisionExit, OnTriggerEnter, and OnTriggerExit. It can also be extended to include additional functionality, such as raycasting or spherecasting to detect nearby objects or surfaces.
/// </summary>


public class CharacterSensor : MonoBehaviour , ICharacterSensor
{
    // ========================
    // PUBLIC STATE
    // ========================
    public bool IsGrounded { get; private set; }
    public bool HasCoyoteTime => groundedRemember > 0f;
    public bool IsTouchingWall { get; private set; }
    public bool IsCeiling { get; private set; }
    
    public bool IsWallLeft { get; private set; }
    public bool IsWallRight { get; private set; }

    public bool IsOnSlope => IsGrounded && GroundNormal.y < 0.99f && GroundNormal.y >= minGroundDot;

    public Vector2 GroundNormal { get; private set; } = Vector2.up;
    public RaycastHit2D GroundHit { get; private set; }

    // ========================
    // CONFIG
    // ========================
    [Header("Layers")]
    [SerializeField] private LayerMask groundLayer;

    [Header("Detection Distances")]
    [SerializeField] private float groundDistance = 0.1f;
    [SerializeField] private float wallDistance = 0.1f;
    [SerializeField] private float ceilingDistance = 0.1f;

    [Header("Capsule Settings")]
    [Tooltip("Thu nhỏ vùng quét để tránh ma sát ảo với tường khi đang di chuyển")]
    [SerializeField, Range(0.5f, 0.95f)] private float castShrink = 0.85f;

    [Header("Refinement")]
    [SerializeField] private float coyoteTimeDuration = 0.15f;
    [SerializeField, Range(0f, 1f)] private float minGroundDot = 0.7f;

    // ========================
    // INTERNAL
    // ========================
    private CapsuleCollider2D col;
    private float groundedRemember;
    public int FacingDir { get; private set; } = 1;

    private void Awake() => col = GetComponent<CapsuleCollider2D>();

    private void FixedUpdate()
    {
        // Sử dụng offset để đảm bảo tâm quét khớp với tâm Collider
        Vector2 pos = (Vector2)transform.position + col.offset;
        Vector2 size = col.size * castShrink;

        UpdateGround(pos, size);
        UpdateWall(pos, size);
        UpdateCeiling(pos, size);
    }

    private void UpdateGround(Vector2 pos, Vector2 size)
    {
        var hit = Physics2D.CapsuleCast(pos, size, col.direction, 0f, Vector2.down, groundDistance, groundLayer);
        bool physicallyGrounded = hit.collider != null && IsValidGround(hit);

        if (physicallyGrounded)
        {
            IsGrounded = true;
            groundedRemember = coyoteTimeDuration;
            GroundHit = hit;
            GroundNormal = hit.normal;
        }
        else
        {
            IsGrounded = false;
            groundedRemember = Mathf.Max(0, groundedRemember - Time.fixedDeltaTime);
            if (!HasCoyoteTime)
            {
                GroundNormal = Vector2.up;
                GroundHit = default;
            }
        }
    }

    private void UpdateWall(Vector2 pos, Vector2 size)
    {
        // Quét đồng thời cả 2 bên để hỗ trợ Wall Jump/Slide chính xác hơn
        var hitRight = Physics2D.CapsuleCast(pos, size, col.direction, 0f, Vector2.right, wallDistance, groundLayer);
        var hitLeft = Physics2D.CapsuleCast(pos, size, col.direction, 0f, Vector2.left, wallDistance, groundLayer);

        IsWallRight = hitRight.collider != null && IsValidWall(hitRight);
        IsWallLeft = hitLeft.collider != null && IsValidWall(hitLeft);
        IsTouchingWall = IsWallRight || IsWallLeft;
    }

    private void UpdateCeiling(Vector2 pos, Vector2 size)
    {
        var hit = Physics2D.CapsuleCast(pos, size, col.direction, 0f, Vector2.up, ceilingDistance, groundLayer);
        // Trần nhà chỉ hợp lệ nếu normal hướng xuống
        IsCeiling = hit.collider != null && hit.normal.y < -0.1f;
    }

    private bool IsValidGround(RaycastHit2D hit) => hit.normal.y >= minGroundDot;
    
    private bool IsValidWall(RaycastHit2D hit) 
    {
        // Một bề mặt là tường nếu nó có độ đứng (x) cao và không phải là sàn/trần (y gần bằng 0)
        return Mathf.Abs(hit.normal.x) >= 0.7f && Mathf.Abs(hit.normal.y) < minGroundDot;
    }

    public void SetFacingDirection(float moveInput)
    {
        if (Mathf.Abs(moveInput) > 0.01f) FacingDir = moveInput > 0 ? 1 : -1;
    }

    // ========================
    // DEBUG GIZMOS
    // ========================
    private void OnDrawGizmosSelected()
    {
        if (!col) col = GetComponent<CapsuleCollider2D>();
        if (!col) return;

        Vector2 pos = (Vector2)transform.position + col.offset;
        Vector2 size = col.size * castShrink;

        DrawCapsuleGizmo(pos, size, Vector2.down, groundDistance, IsGrounded ? Color.green : Color.gray);
        DrawCapsuleGizmo(pos, size, Vector2.right, wallDistance, IsWallRight ? Color.blue : Color.gray);
        DrawCapsuleGizmo(pos, size, Vector2.left, wallDistance, IsWallLeft ? Color.blue : Color.gray);
        DrawCapsuleGizmo(pos, size, Vector2.up, ceilingDistance, IsCeiling ? Color.red : Color.gray);
    }

    private void DrawCapsuleGizmo(Vector2 pos, Vector2 size, Vector2 direction, float distance, Color color)
    {
        Gizmos.color = color;
        Vector2 endPoint = pos + direction.normalized * distance;
        Gizmos.DrawLine(pos, endPoint);
        Gizmos.DrawWireCube(endPoint, size);
    }
}
