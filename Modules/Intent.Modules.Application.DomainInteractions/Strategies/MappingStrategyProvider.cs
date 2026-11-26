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
            Logging.Log.Debug($"Multiple mapping strategies matched for {Describe(method)}: [{string.Join(", ", foundStrategies)}]");
        }

        return foundStrategies.Count > 0;
    }

    /// <summary>
    /// Returns the highest-priority matching strategy, or <see langword="null"/> when no mapping module is installed.
    /// A null result is a valid answer: callers decide whether a missing mapper is an error.
    /// </summary>
    public IMappingStrategy? GetMappingStrategy(ICSharpClassMethodDeclaration method)
    {
        var foundStrategies = _strategies.OrderBy(x => x.Priority).Where(x => x.Strategy.IsMatch(method)).ToList();
        if (foundStrategies.Count > 1)
        {
            Logging.Log.Debug($"Multiple mapping strategies found for {Describe(method)}: [{string.Join(", ", foundStrategies)}]");
        }
        return foundStrategies.FirstOrDefault()?.Strategy;
    }

    // The method builder's ToString() renders its full C#, so name it explicitly.
    private static string Describe(ICSharpClassMethodDeclaration method) => $"{method.Class.Name}.{method.Name}";
}
