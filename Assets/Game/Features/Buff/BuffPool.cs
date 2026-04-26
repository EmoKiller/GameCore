// using System.Collections.Generic;
// using UnityEngine;
// namespace Game.Character.Core.Stats
// {
//     public sealed class BuffPool
//     {
//         private readonly Stack<RuntimeBuff> _pool = new();

//         public RuntimeBuff Get(BuffDefinition def, string sourceId)
//         {
//             if (_pool.Count > 0)
//             {
//                 var buff = _pool.Pop();
//                 buff.Reset();
//                 return buff;
//             }

//             return new RuntimeBuff(def, sourceId);
//         }

//         public void Release(RuntimeBuff buff)
//         {
//             _pool.Push(buff);
//         }
//     }
// }