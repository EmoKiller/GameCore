
public interface ICombatContext
{
    void DealDamage(int amount);
    void Heal(int amount);
    void AddShield(int amount);
    void AddResource(int amount);
}
public sealed class NullCombatContext : ICombatContext
{
    public void DealDamage(int amount) { }

    public void Heal(int amount) { }

    public void AddShield(int amount) { }

    public void AddResource(int amount) { }
}