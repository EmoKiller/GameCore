using Game.Application.Events;
using UnityEngine;

public sealed class ResolveMatchesUseCase
{
    private readonly IMatchResolutionSystem _resolver;
    private readonly IMatchEffectProcessor _effectProcessor;
    private readonly ICombatContext _combatContext;
    private readonly IEventBus _eventBus;

    public ResolveMatchesUseCase(
        IMatchResolutionSystem resolver,
        IMatchEffectProcessor effectProcessor,
        ICombatContext combatContext,
        IEventBus eventBus)
    {
        _resolver = resolver;
        _effectProcessor = effectProcessor;
        _combatContext = combatContext;
        _eventBus = eventBus;
    }

    public void Execute(IGrid grid)
    {
        var matches = _resolver.Resolve(grid);

        if (matches.Count == 0)
            return;

        _effectProcessor.Process(matches, _combatContext);

        _eventBus.Publish(new MatchesResolvedEvent(matches) , EventChannel.System);
    }
}