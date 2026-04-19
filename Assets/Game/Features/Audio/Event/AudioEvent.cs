using Game.Application.Events;
using UnityEngine;
namespace Audio.Event
{
    public struct UI_Clicked : IEvent { }
    public struct Enemy_Died : IEvent { }
    public struct Music_ChangeState : IEvent
    {
        public string State;
    }
}

