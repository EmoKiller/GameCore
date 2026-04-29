
/// <summary>
/// Interface cho các object có thể bị damage
/// Segregated interface - chỉ xử lý health
/// </summary>
public interface IDamageable
{
    /// <summary>Nhận damage</summary>
    void TakeDamage(float damage);
    
    /// <summary>Lấy máu</summary>
    void Heal(float amount);
    
    /// <summary>Lấy máu hiện tại</summary>
    float GetCurrentHealth();
    
    /// <summary>Lấy máu tối đa</summary>
    float GetMaxHealth();
    
    /// <summary>Kiểm tra còn sống hay không</summary>
    bool IsAlive();
}
