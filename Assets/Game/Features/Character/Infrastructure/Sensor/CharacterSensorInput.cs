using UnityEngine;

public struct CharacterSensorInput
{
    public bool HasGroundHit;
    public Vector2 GroundNormal;

    public bool HasWallLeft;
    public bool HasWallRight;

    public bool HasCeilingHit;
}
