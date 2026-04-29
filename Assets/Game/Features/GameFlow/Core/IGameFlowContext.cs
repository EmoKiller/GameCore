using UnityEngine;

public interface IGameFlowContext
{
    void SetNextState(EGameState state);
}
