
public interface ICombatContext
{
    void DealDamage(int amount);
    void Heal(int amount);
    void AddShield(int amount);
    void AddResource(int amount);
}
