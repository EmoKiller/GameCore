using System;
using UnityEngine;

namespace Game.Application.Core.Input
{
    public interface IGameplayInputService
    {
        Vector2 MoveInput { get; }

        event Action<Vector2> OnMove;
        event Action OnJump;
        event Action OnAttack;

        void SetMove(Vector2 value);
        void TriggerJump();
        void TriggerAttack();
    }
}
