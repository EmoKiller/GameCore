using UnityEngine;


/// <summary>
/// A component that can be used to detect collisions and triggers for a character.
///  It can be used to detect when the character is grounded, when it is touching a wall,
///  or when it is in the air. It can also be used to detect when the character is colliding with other objects, such as enemies or pickups.
/// The CharacterSensor can be used in conjunction with a CharacterController to provide more detailed information about the character's state and interactions with the environment. It can be used to implement features such as wall jumping, ledge grabbing, or detecting when the character is in a specific area (e.g., a trigger zone).
/// The CharacterSensor can be implemented using Unity's built-in collision and trigger detection methods, such as OnCollisionEnter, OnCollisionExit, OnTriggerEnter, and OnTriggerExit. It can also be extended to include additional functionality, such as raycasting or spherecasting to detect nearby objects or surfaces.
/// </summary>


public sealed class CharacterSensor : ICharacterSensor
{
    // ========================
    // STATE
    // ========================
    public bool IsGrounded { get; private set; }
    public bool HasCoyoteTime => _groundedRemember > 0f;

    public bool IsTouchingWall { get; private set; }
    public bool IsCeiling { get; private set; }

    public bool IsWallLeft { get; private set; }
    public bool IsWallRight { get; private set; }

    public bool IsOnSlope =>
        IsGrounded &&
        GroundNormal.y < 0.99f &&
        GroundNormal.y >= _minGroundDot;

    public Vector2 GroundNormal { get; private set; } = Vector2.up;

    public int FacingDir { get; private set; } = 1;

    // ========================
    // CONFIG (Injectable)
    // ========================
    private readonly float _coyoteTimeDuration;
    private readonly float _minGroundDot;

    // ========================
    // INTERNAL
    // ========================
    private float _groundedRemember;

    // ========================
    // CONSTRUCTOR
    // ========================
    public CharacterSensor(float coyoteTimeDuration, float minGroundDot)
    {
        _coyoteTimeDuration = coyoteTimeDuration;
        _minGroundDot = minGroundDot;
    }

    // ========================
    // UPDATE (CORE LOGIC ONLY)
    // ========================
    public void Update(in CharacterSensorInput input, float deltaTime)
    {
        UpdateGround(input, deltaTime);
        UpdateWall(input);
        UpdateCeiling(input);
    }

    private void UpdateGround(in CharacterSensorInput input, float deltaTime)
    {
        bool physicallyGrounded =
            input.HasGroundHit &&
            IsValidGround(input.GroundNormal);

        if (physicallyGrounded)
        {
            IsGrounded = true;
            _groundedRemember = _coyoteTimeDuration;
            GroundNormal = input.GroundNormal;
        }
        else
        {
            IsGrounded = false;
            _groundedRemember = Mathf.Max(0f, _groundedRemember - deltaTime);

            if (!HasCoyoteTime)
            {
                GroundNormal = Vector2.up;
            }
        }
    }

    private void UpdateWall(in CharacterSensorInput input)
    {
        IsWallLeft = input.HasWallLeft;
        IsWallRight = input.HasWallRight;

        IsTouchingWall = IsWallLeft || IsWallRight;
    }

    private void UpdateCeiling(in CharacterSensorInput input)
    {
        IsCeiling = input.HasCeilingHit;
    }

    // ========================
    // VALIDATION
    // ========================
    private bool IsValidGround(Vector2 normal)
    {
        return normal.y >= _minGroundDot;
    }

    // ========================
    // INPUT
    // ========================
    public void SetFacingDirection(float moveInput)
    {
        if (Mathf.Abs(moveInput) > 0.01f)
        {
            FacingDir = moveInput > 0 ? 1 : -1;
        }
    }
}