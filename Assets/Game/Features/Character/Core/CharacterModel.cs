
using System;
using Game.Character.Core.Stats;

namespace Game.Character.Core
{
    
    public sealed class CharacterModel 
    {
        private readonly ICharacterStat _stats;
        
        /// <summary>
        /// Initializes a new instance of the CharacterModel.
        /// </summary>
        public CharacterModel(ICharacterStat stats)
        {
            _stats = stats;
        }
        public IReadOnlyCharacterStats Stats => _stats.Read;

    }
}
