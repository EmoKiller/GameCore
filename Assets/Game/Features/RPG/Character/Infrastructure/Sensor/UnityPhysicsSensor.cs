using UnityEngine;
public interface IPhysicsSensor
{
    CharacterSensorInput Sample();
}
[RequireComponent(typeof(CapsuleCollider2D))]
public sealed class UnityPhysicsSensor : MonoBehaviour, IPhysicsSensor
{
    // ========================
    // CONFIG
    // ========================
    [Header("Layers")]
    [SerializeField] private LayerMask groundLayer;

    [Tooltip("Nếu không set sẽ fallback về groundLayer")]
    [SerializeField] private LayerMask wallLayer;

    [Header("Detection Distances")]
    [SerializeField] private float groundDistance = 0.1f;
    [SerializeField] private float wallDistance = 0.1f;
    [SerializeField] private float ceilingDistance = 0.1f;

    [Header("Capsule Settings")]
    [SerializeField, Range(0.5f, 0.95f)]
    private float castShrink = 0.85f;

    [Header("Refinement")]
    [SerializeField, Range(0f, 1f)]
    private float minGroundDot = 0.7f;

    // ========================
    // INTERNAL
    // ========================
    private CapsuleCollider2D _col;

    private LayerMask EffectiveWallLayer =>
        wallLayer.value == 0 ? groundLayer : wallLayer;

    private void Awake()
    {
        _col = GetComponent<CapsuleCollider2D>();
    }

    // ========================
    // MAIN API
    // ========================
    public CharacterSensorInput Sample()
    {
        Vector2 pos = (Vector2)transform.position + _col.offset;
        Vector2 size = _col.size * castShrink;

        return new CharacterSensorInput
        {
            HasGroundHit = CheckGround(pos, size, out Vector2 groundNormal),
            GroundNormal = groundNormal,

            HasWallLeft = CheckWall(pos, size, Vector2.left),
            HasWallRight = CheckWall(pos, size, Vector2.right),

            HasCeilingHit = CheckCeiling(pos, size)
        };
    }

    // ========================
    // GROUND
    // ========================
    private bool CheckGround(Vector2 pos, Vector2 size, out Vector2 normal)
    {
        var hit = Physics2D.CapsuleCast(
            pos,
            size,
            _col.direction,
            0f,
            Vector2.down,
            groundDistance,
            groundLayer
        );

        if (hit.collider != null && IsValidGround(hit))
        {
            normal = hit.normal;
            return true;
        }

        normal = Vector2.up;
        return false;
    }

    // ========================
    // WALL (FIXED)
    // ========================
    private bool CheckWall(Vector2 pos, Vector2 size, Vector2 dir)
    {
        var hit = Physics2D.CapsuleCast(
            pos,
            size,
            _col.direction,
            0f,
            dir,
            wallDistance,
            EffectiveWallLayer // ✅ FIX
        );

        return hit.collider != null && IsValidWall(hit);
    }

    // ========================
    // CEILING
    // ========================
    private bool CheckCeiling(Vector2 pos, Vector2 size)
    {
        var hit = Physics2D.CapsuleCast(
            pos,
            size,
            _col.direction,
            0f,
            Vector2.up,
            ceilingDistance,
            groundLayer
        );

        return hit.collider != null && hit.normal.y < -0.1f;
    }

    // ========================
    // VALIDATION
    // ========================
    private bool IsValidGround(RaycastHit2D hit)
    {
        return hit.normal.y >= minGroundDot;
    }

    private bool IsValidWall(RaycastHit2D hit)
    {
        return Mathf.Abs(hit.normal.x) >= 0.7f &&
               Mathf.Abs(hit.normal.y) < minGroundDot;
    }
}