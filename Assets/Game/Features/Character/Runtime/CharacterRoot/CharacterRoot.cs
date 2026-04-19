using Game.Character.Core;
using Game.Share.StateMachine;
using UnityEngine;

/// <summary>
/// CharacterRoot là MonoBehaviour chính cho Character,
/// Chịu trách nhiệm:
/// - giữ reference tới components như AnimationController, MovementComponent, và CharacterModel
/// - khởi tạo CharacterModel từ CharacterStatsConfig (Dependency Injection)
/// - triển khai các interface IDamageable và IMovable bằng cách gọi model và movementComponent
/// </summary>


public abstract class CharacterRoot : MonoBehaviour , IDamageable
{
    /// <summary>
    /// Model chứa dữ liệu và logic liên quan đến trạng thái của Character,
    /// được khởi tạo từ config thông qua Dependency Injection.
    /// </summary>
    protected CharacterModel model;
    public CharacterContext CharacterContext{ get; private set; }
    void Awake()
    {
        var movement = GetComponent<ICharacterMovement>();
        var animator = GetComponent<ICharacterAnimator>();
        if (movement == null)
        {
            Debug.LogError($"ICharacterMovement missing on {gameObject.name}");
            return;
        }
        if (animator == null)
        {
            Debug.LogError($"ICharacterAnimator missing on {gameObject.name}");
            return;
        }
        CharacterContext = new CharacterContext(movement, animator);
    }

    #region Initialization

    protected virtual void InitializeConfig(CharacterStatsConfig config)
    {
        if (config == null)
        {
            Debug.LogError($"CharacterStatsConfig missing on {gameObject.name}");
            return;
        }

        model = new CharacterModel(config);
        model.OnDeath += OnDeath;
    }

#endregion

#region IDamageable 

    // IDamageable Implementation
    public virtual void TakeDamage(float damage)
    {
        model.ApplyDamage(damage);
    }
    
    public virtual void Heal(float amount)
    {
        model.ApplyHeal(amount);
    }
    
    public float GetCurrentHealth()
    {
        return model.GetCurrentHealth();
    }
    
    public float GetMaxHealth()
    {
        return model.GetMaxHealth();
    }
    
    public bool IsAlive()
    {
        return model.IsAlive();
    }
#endregion

#region Death
    
    

    /// <summary>Gọi khi character chết</summary>
    protected virtual void OnDeath()
    {
        gameObject.SetActive(false);
    }

#endregion


}
