// using System;
// using UnityEngine;
// namespace Game.Character.Core.Stats
// {
//     public interface IRuntimeBuff : IStatModifierSource
//     {
//         bool IsFinished { get; }

//         void OnApply(CharacterStats stats);
//         void OnUpdate(float deltaTime);
//         void OnRemove(CharacterStats stats);
//     }
//     public sealed class RuntimeBuff : IRuntimeBuff
//     {
//         private readonly BuffDefinition _definition;
//         public BuffDefinition Definition => _definition;
//         private readonly StatModifier[] _modifiers;

//         private float _remainingTime;
//         private int _stackCount;
//         private bool _isFinished;

//         public string SourceId { get; }

//         public bool IsFinished => _isFinished;

//          public RuntimeBuff(BuffDefinition definition, string sourceId)
//         {
//             _definition = definition ?? throw new ArgumentNullException(nameof(definition));
//             SourceId = sourceId ?? throw new ArgumentNullException(nameof(sourceId));

//             int count = definition.Modifiers.Length;
//             _modifiers = new StatModifier[count];

//             for (int i = 0; i < count; i++)
//             {
//                 var m = definition.Modifiers[i];
//                 _modifiers[i] = new StatModifier(m.Value, m.ModifierType, this);
//             }

//             _stackCount = 1;
//             _remainingTime = definition.Duration;
//         }

//         public void OnApply(CharacterStats stats)
//         {
//             ApplyModifiers(stats, 1);
//         }
//         public void OnRemove(CharacterStats stats)
//         {
//             ApplyModifiers(stats, -_stackCount);
//         }
//         private void ApplyModifiers(CharacterStats stats, int multiplier)
//         {
//             for (int i = 0; i < _modifiers.Length; i++)
//             {
//                 // var def = _definition.Modifiers[i];
//                 // var stat = stats.GetStatInternal(def.StatType);

//                 // if (multiplier > 0)
//                 // {
//                 //     stat.AddModifier(_modifiers[i]);
//                 // }
//                 // else
//                 // {
//                 //     stat.RemoveModifier(_modifiers[i]);
//                 // }
//             }
//         }
//         public void AddStack(CharacterStats stats)
//         {
//             if (_stackCount >= _definition.MaxStacks)
//                 return;

//             _stackCount++;

//             // apply thêm modifier (stack)
//             ApplyModifiers(stats, 1);
//         }

//         public void OnUpdate(float deltaTime)
//         {
//             if (_isFinished)
//                 return;

//             if (_definition.Duration <= 0f)
//                 return; // infinite buff

//             _remainingTime -= deltaTime;

//             if (_remainingTime <= 0f)
//             {
//                 _isFinished = true;
//             }
//         }

//         public void Refresh()
//         {
//             _remainingTime = _definition.Duration;
//         }

//         public void Reset()
//         {
//             _remainingTime = _definition.Duration;
//             _isFinished = false;
//         }
//     }
// }