using System;
using UnityEngine;
namespace Game.Character.Core.Stats
{
    public interface ICharacterStat
    {
        IReadOnlyCharacterStats Read { get; }
        ICharacterStatsWriter Write { get; }
    }
    public sealed class CharacterStats : ICharacterStat
    {
        public IReadOnlyCharacterStats Read { get; }
        public ICharacterStatsWriter Write { get; }

        public CharacterStats(
            IReadOnlyCharacterStats read,
            ICharacterStatsWriter write)
        {
            Read = read ?? throw new ArgumentNullException(nameof(read));
            Write = write ?? throw new ArgumentNullException(nameof(write));
        }
    }
}
