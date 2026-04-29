using UnityEngine;
public interface IMatchEffect
{
    void Apply(in Match match, ICombatContext context);
}
public sealed class SwordEffect : IMatchEffect
{
    public void Apply(in Match match, ICombatContext context)
    {
        int damage = match.Positions.Count * 10;
        context.DealDamage(damage);
    }
}
public sealed class HeartEffect : IMatchEffect
{
    public void Apply(in Match match, ICombatContext context)
    {
        int heal = match.Positions.Count * 5;
        context.Heal(heal);
    }
}
public sealed class ShieldEffect : IMatchEffect
{
    public void Apply(in Match match, ICombatContext context)
    {
        int shield = match.Positions.Count * 3;
        context.AddShield(shield);
    }
}
public sealed class CoinEffect : IMatchEffect
{
    public void Apply(in Match match, ICombatContext context)
    {
        context.AddResource(match.Positions.Count);
    }
}