using Game.Character.Core;
using UnityEngine;
public interface ICharacterBehaviorFactory
{
    ICharacterBehavior Create(ICharacterActions context);
}
// public sealed class PlayerBehaviorFactory : ICharacterBehaviorFactory
// {
//     public ICharacterBehavior Create(ICharacterActions context)
//     {
//         return new PlayerStateSystem(context);
//     }
// }
// public sealed class EnemyBehaviorFactory : ICharacterBehaviorFactory
// {
//     public ICharacterBehavior Create(CharacterContext context)
//     {
//         return new EnemyAIStateSystem(context);
//     }
// }
