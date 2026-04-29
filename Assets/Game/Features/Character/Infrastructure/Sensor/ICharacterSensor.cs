using UnityEngine;

public interface ICharacterSensor
{
    bool IsGrounded { get; }
    bool HasCoyoteTime { get; }
    bool IsTouchingWall { get; }
    bool IsCeiling { get; }

    bool IsWallLeft { get; }
    bool IsWallRight { get; }

    bool IsOnSlope { get; }

    Vector2 GroundNormal { get; }

    int FacingDir { get; }

    void Update(in CharacterSensorInput input, float deltaTime);
    void SetFacingDirection(float moveInput);
}
