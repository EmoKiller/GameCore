using Game.Character.Core.Stats;
using UnityEngine;
public interface ICharacterStatsFacade
{
    float GetMoveSpeed();
    float GetAttack();
    float GetDefense();

    bool TryConsumeStamina(float amount);
}
public class CharacterStatsFacade : ICharacterStatsFacade
{
    private readonly IReadOnlyCharacterStats _Read;
    private readonly ICharacterStatsWriter _writer;

    public CharacterStatsFacade(
        IReadOnlyCharacterStats read,
        ICharacterStatsWriter writer)
    {
        _Read = read;
        _writer = writer;
    }

    public float GetMoveSpeed()
    {
        return _Read.GetStat(EStatType.MoveSpeed).FinalValue;
    }

    public float GetAttack()
    {
        return _Read.GetStat(EStatType.Attack).FinalValue;
    }

    public float GetDefense()
    {
        return _Read.GetStat(EStatType.Defense).FinalValue;
    }

    public bool TryConsumeStamina(float amount)
    {
        var res = _Read.GetResource(EResourceType.Stamina);

        if (res.Current < amount)
            return false;

        _writer.ConsumeResource(EResourceType.Stamina, amount);
        return true;
    }
}
