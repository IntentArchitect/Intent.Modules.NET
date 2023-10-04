using System.Linq;
using Intent.Engine;
using Intent.Modules.Application.MediatR.CRUD.Decorators;
using Intent.Modules.Application.MediatR.Templates.CommandHandler;
using Intent.Modules.Application.MediatR.Templates.QueryHandler;
using Intent.Utils;

namespace Intent.Modules.Application.MediatR.CRUD.CrudStrategies;

internal static class StrategyFactory
{
    public static ICrudImplementationStrategy GetMatchedCommandStrategy(CommandHandlerTemplate template)
    {
        var strategies = new ICrudImplementationStrategy[]
        {
            new CreateImplementationStrategy(template),
            new UpdateImplementationStrategy(template),
            new DeleteImplementationStrategy(template),
            new DomainCtorImplementationStrategy(template),
            new DomainOpImplementationStrategy(template)
        };

        var matchedStrategies = strategies.Where(strategy => strategy.IsMatch()).ToArray();
        if (matchedStrategies.Length == 1)
        {
            return matchedStrategies[0];
        }
        else if (matchedStrategies.Length > 1)
        {
            Logging.Log.Warning($@"Multiple CRUD implementation strategies were found that can implement this Command [{template.Model.Name}]");
            Logging.Log.Debug($@"Strategies: {string.Join(", ", matchedStrategies.Select(s => s.GetType().Name))}");
        }

        return null;
    }

    public static ICrudImplementationStrategy GetMatchedQueryStrategy(QueryHandlerTemplate template, IApplication application)
    {
        var strategies = new ICrudImplementationStrategy[]
        {
            new GetAllImplementationStrategy(template, application),
            new GetByIdImplementationStrategy(template, application),
            new GetAllPaginationImplementationStrategy(template)
        };

        var matchedStrategies = strategies.Where(strategy => strategy.IsMatch()).ToArray();
        if (matchedStrategies.Length == 1)
        {
            return matchedStrategies[0];
        }
        else if (matchedStrategies.Length > 1)
        {
            Logging.Log.Warning($@"Multiple CRUD implementation strategies were found that can implement this Query [{template.Model.Name}]");
            Logging.Log.Debug($@"Strategies: {string.Join(", ", matchedStrategies.Select(s => s.GetType().Name))}");
        }

        return null;
    }
}