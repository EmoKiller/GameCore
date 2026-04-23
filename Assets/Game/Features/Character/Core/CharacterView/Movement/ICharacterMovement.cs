using UnityEngine;

/// <summary>
/// Interface for movement input, defining methods for setting speed, moving in a direction, and resetting movement.
/// Giao diện cho input di chuyển, định nghĩa các phương thức để thiết lập tốc độ, di chuyển theo hướng và reset di chuyển.
/// </summary>
public interface ICharacterMovement
{
    /// <summary>Vận tốc hiện tại (physics)</summary>
    Vector3 ActualVelocity  { get; }

    /// <summary>Tốc độ hiện tại (magnitude của velocity)</summary>
    float CurrentSpeed { get; }


    float TargetVelocityX { get; }

    // ===== INPUT / CONTROL =====

    /// <summary>Thiết lập input di chuyển (-1 → 1)</summary>
    void SetMoveInput(float input);

    /// <summary>Di chuyển trực tiếp (ít dùng, thường cho debug/AI)</summary>
    void Move(Vector3 direction);

    /// <summary>Reset input (idle)</summary>
    void ResetMovement();


    // ===== ACTION =====

    /// <summary>Thực hiện nhảy</summary>
    void Jump();

    void SetSprint(bool value);


    // ===== UNITY LOOP =====
}
