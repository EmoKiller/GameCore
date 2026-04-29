using UnityEngine;

public enum EAnimationEventType
{
    /// <summary>
    /// enable hitbox
    /// </summary>
    AttackStart = 0, 

    /// <summary>
    /// apply damage
    /// </summary>
    AttackHit = 1,
     
    /// <summary>
    /// disable hitbox  
    /// </summary>
    AttackEnd = 2,   
}
