using UnityEngine;

/// <summary>
/// Xử lý health của character
/// Single Responsibility - chỉ quản lý HP
/// Dependency Injection - được init từ Character base
/// </summary>
public class HealthComponent : MonoBehaviour
{
    private float currentHealth;
    private float maxHealth;
    
    // Event cho các hệ thống khác lắng nghe
    public delegate void HealthChangedDelegate(float current, float max);
    public event HealthChangedDelegate OnHealthChanged;
    
    public delegate void DeadDelegate();
    public event DeadDelegate OnDeath;
    
    /// <summary>Initialize health</summary>
    public void Initialize(float maxHp)
    {
        maxHealth = maxHp;
        currentHealth = maxHealth;
    }
    
    /// <summary>Nhận damage</summary>
    public void TakeDamage(float damage)
    {
        if (!IsAlive()) return;
        
        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);
        
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        
        if (!IsAlive())
        {
            OnDeath?.Invoke();
        }
    }
    
    /// <summary>Lấy máu</summary>
    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
    
    /// <summary>Lấy máu hiện tại</summary>
    public float GetCurrentHealth() => currentHealth;
    
    /// <summary>Lấy máu tối đa</summary>
    public float GetMaxHealth() => maxHealth;
    
    /// <summary>Kiểm tra còn sống</summary>
    public bool IsAlive() => currentHealth > 0;
    
    /// <summary>Lấy % máu</summary>
    public float GetHealthPercent() => maxHealth > 0 ? currentHealth / maxHealth : 0f;
}
