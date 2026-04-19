

namespace Game.Character.Core.Stats
{
    /// <summary>
    /// Represents the health resource of a character.
    /// </summary>
    public sealed class Stamina : Resource
    {
        
        /// <summary>
        /// Initializes a new instance of the Stamina class.
        /// </summary>
        /// <param name="maxStamina">The maximum stamina value.</param>
        public Stamina(StatValue maxStamina) : base(maxStamina)
        {
        }
    }
}
