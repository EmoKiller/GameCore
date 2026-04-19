using UnityEngine;

public interface ICharacterSensor
{
    bool IsGrounded { get; }
    bool IsTouchingWall { get; }
    bool IsCeiling { get; }
    bool IsWallLeft { get; }
    bool IsWallRight { get; }
    bool IsOnSlope { get; }
}
