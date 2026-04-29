// using Game.Application.Events;
// using UnityEngine;
// namespace Game.Character.Core.Stats
// {
//     public sealed class BuffSystem
//     {
//         private readonly CharacterStats _stats;

//         private readonly IRuntimeBuff[] _buffs;
//         private int _count;

//         public BuffSystem(CharacterStats stats, int capacity = 16)
//         {
//             _stats = stats;
//             _buffs = new IRuntimeBuff[capacity];
//             _count = 0;
//         }

//         public void AddBuff(RuntimeBuff newBuff)
//         {
//             for (int i = 0; i < _count; i++)
//             {
//                 var existing = _buffs[i] as RuntimeBuff;

//                 if (existing.SourceId == newBuff.SourceId)
//                 {
//                     HandleStacking(existing, newBuff);
//                     return;
//                 }
//             }

//             // add mới
//             _buffs[_count++] = newBuff;
//             newBuff.OnApply(_stats);
//         }
//         private void HandleStacking(RuntimeBuff existing, RuntimeBuff incoming)
//         {
//             var def = existing.Definition;

//             switch (def.StackingPolicy)
//             {
//                 case EBuffStackingPolicy.None:
//                     return;

//                 case EBuffStackingPolicy.Refresh:
//                     existing.Refresh();
//                     return;

//                 case EBuffStackingPolicy.Stack:
//                     existing.AddStack(_stats);
//                     existing.Refresh();
//                     return;

//                 case EBuffStackingPolicy.Replace:
//                     existing.OnRemove(_stats);
//                     existing.Reset();

//                     // overwrite
//                     existing = incoming;
//                     existing.OnApply(_stats);
//                     return;
//             }
//         }

//         public void Update(float deltaTime)
//         {
//             for (int i = 0; i < _count; i++)
//             {
//                 var buff = _buffs[i];

//                 buff.OnUpdate(deltaTime);

//                 if (buff.IsFinished)
//                 {
//                     RemoveAt(i);
//                     i--;
//                 }
//             }
//         }
//         public void RaiseEvent(IEvent gameEvent)
//         {
//             for (int i = 0; i < _count; i++)
//             {
//                 if (_buffs[i] is IEventDrivenBuff e)
//                 {
//                     e.OnEvent(gameEvent, _stats);
//                 }
//             }
//         }
//         private void RemoveAt(int index)
//         {
//             var buff = _buffs[index];

//             buff.OnRemove(_stats);

//             int last = _count - 1;
//             _buffs[index] = _buffs[last];
//             _buffs[last] = null;

//             _count--;
//         }
//     }
// }
