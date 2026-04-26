// using Game.Application.Events;
// using Game.Character.Core.Stats;
// using UnityEngine;

// public interface IEventDrivenBuff
// {
//     void OnEvent(IEvent gameEvent, CharacterStats stats);
// }
// // public sealed class LifestealBuff : RuntimeBuff, IEventDrivenBuff
// // {
// //     private readonly float _ratio;

// //     public LifestealBuff(
// //         BuffDefinition def,
// //         string sourceId,
// //         float ratio)
// //         : base(def, sourceId)
// //     {
// //         _ratio = ratio;
// //     }

// //     public void OnEvent(IEvent gameEvent, CharacterStats stats)
// //     {
// //         if (gameEvent is DamageEvent dmg)
// //         {
// //             var heal = dmg.Damage * _ratio;

// //             var hp = stats.GetResourceInternal(EResourceType.Health);
// //             hp.Restore(heal);
// //         }
// //     }
// // }
