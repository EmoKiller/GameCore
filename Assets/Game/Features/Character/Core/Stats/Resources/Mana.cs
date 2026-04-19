

namespace Game.Character.Core.Stats
{
    /// <summary>
    /// Represents the health resource of a character.
    /// </summary>
    public sealed class Mana : Resource
    {
        
        /// <summary>
        /// Initializes a new instance of the Health class.
        /// </summary>
        /// <param name="maxHealth">The maximum health value.</param>
        public Mana(StatValue maxHealth) : base(maxHealth)
        {
        }
    }
}
