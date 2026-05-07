using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Area Clear",menuName = "Puzzle/Special/Special Behaviours/Area Clear")]
public sealed class AreaClearBehaviour : SpecialTileBehaviour
{
    [SerializeField]
    private int _radius = 1;

    [SerializeField]
    private int _charges = 2;
    public override SpecialActivationResult Activate(
        PuzzleBoard board,
        TileData tile,
        TilePosition position,
        BoardChangeSet changeSet)
    {
        TileSpecialData specialData = tile.Special;

        AreaClearRuntimeState runtime = tile.RuntimeSpecialState as AreaClearRuntimeState;

        runtime.RemainingCharges--;

        List<TilePosition> triggered = new List<TilePosition>();

        for (int x = -_radius; x <= _radius; x++)
        {
            for (int y = -_radius; y <= _radius; y++)
            {
                TilePosition target =
                    new TilePosition(
                        position.X + x,
                        position.Y + y);

                SpecialActivationUtility.ProcessTarget(
                    board,
                    target,
                    changeSet,
                    triggered);
            }
        }

        if (runtime.RemainingCharges <= 0)
        {
            runtime.LifecycleState = ESpecialLifecycleState.Consumed;
            return new SpecialActivationResult(triggered, ESpecialConsumePolicy.Destroy);

        }
        if (specialData.TriggerMode == ESpecialTriggerMode.AutoCascade)
        {
            runtime.LifecycleState = ESpecialLifecycleState.PendingRetrigger;
        }
        else
        {
            runtime.LifecycleState = ESpecialLifecycleState.WaitingForTrigger;
        }
        return new SpecialActivationResult(triggered, ESpecialConsumePolicy.Keep);
    }
    public override TileRuntimeSpecialState CreateRuntimeState()
    {
        return new AreaClearRuntimeState(_charges);
    }
}