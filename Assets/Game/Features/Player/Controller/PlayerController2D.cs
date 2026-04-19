using System;
using UnityEngine;

/// <summary>
/// PlayerController - MonoBehaviour để attach vào Player prefab
/// Single Responsibility - chỉ quản lý input và kết nối với PlayerCharacter
/// </summary>

[RequireComponent(typeof(CharacterMovement2D))]
[RequireComponent(typeof(AnimationController2D))]
[RequireComponent(typeof(PlayerInputAdapter))]
[RequireComponent(typeof(CharacterSensor))]
public class PlayerController2D : CharacterRoot
{
    
    
    void OnEnable()
    {
        
    }

    void OnDisable()
    {
        
    }
    
    protected override void OnDeath()
    {
        // Có thể bắn các event, animation... trước khi deactivate
        Debug.Log("Player died!");
        base.OnDeath();
    }
    
    private void OnDestroy()
    {
        
    }
}
