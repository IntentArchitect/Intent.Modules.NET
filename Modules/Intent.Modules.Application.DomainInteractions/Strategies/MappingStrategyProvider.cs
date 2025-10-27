using Intent.Modules.Common.CSharp.Builder;
using Intent.Utils;
using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.Application.DomainInteractions.Strategies;

public class MappingStrategyProvider
{
    private record StrategyEntry(IMappingStrategy Strategy, int Priority);
    
    private readonly List<StrategyEntry> _strategies = new();

    public static readonly MappingStrategyProvider Instance = new();

    public void Register(IMappingStrategy strategy)
    {
        Register(strategy, 0);
    }

    public void Register(IMappingStrategy strategy, int priority)
    {
        _strategies.Add(new (strategy, priority));
    }

    public bool HasMappingStrategy(ICSharpClassMethodDeclaration method)
    {
        var foundStrategies = _strategies.Where(x => x.Strategy.IsMatch(method)).ToList();
        if (foundStrategies.Count > 1)
        {
            Logging.Log.Debug($"Multiple mapping strategies matched for {method}: [{string.Join(", ", foundStrategies)}]");
        }

        return foundStrategies.Count > 0;
    }

    public IMappingStrategy? GetMappingStrategy(ICSharpClassMethodDeclaration method)
    {
        var foundStrategies = _strategies.OrderBy(x => x.Priority).Where(x => x.Strategy.IsMatch(method)).ToList();
        if (foundStrategies.Count == 0)
        {
            Logging.Log.Warning($"No mapping strategy matched for {method}");
            return null;
        }
        if (foundStrategies.Count > 1)
        {
            Logging.Log.Debug($"Multiple mapping strategies found for {method}: [{string.Join(", ", foundStrategies)}]");
        }
        return foundStrategies[0].Strategy;
    }
}