using UnityEngine;

public enum EBuffStackingPolicy
{
    /// <summary>
    /// không stack, ignore nếu đã có
    /// </summary>
    None,     
    /// <summary>
    /// reset duration      
    /// </summary>
    Refresh,
    /// <summary>
    /// cộng stack        
    /// </summary>
    Stack,  
    /// <summary>
    /// remove old, add new        
    /// </summary>
    Replace,        
}
