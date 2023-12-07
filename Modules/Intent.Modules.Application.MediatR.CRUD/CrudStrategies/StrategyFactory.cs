using System.Linq;
using Intent.Engine;
using Intent.Modules.Application.MediatR.CRUD.CrudMappingStrategies;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.MediatR.CRUD.Decorators;
using Intent.Modules.Application.MediatR.Templates.CommandHandler;
using Intent.Modules.Application.MediatR.Templates.QueryHandler;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Utils;

namespace Intent.Modules.Application.MediatR.CRUD.CrudStrategies;

internal static class StrategyFactory
{
    public static ICrudImplementationStrategy GetMatchedCommandStrategy(CSharpTemplateBase<CommandModel> template)
    {
        var strategies = new ICrudImplementationStrategy[]
        {
            new CommandMappingImplementationStrategy(template),

            new CreateImplementationStrategy(template),
            new UpdateImplementationStrategy(template),
            new DeleteImplementationStrategy(template),
            new DomainCtorImplementationStrategy(template),
            new DomainOpImplementationStrategy(template)
        };

        var matchedStrategies = strategies.Where(strategy => strategy.IsMatch()).ToArray();

        if (matchedStrategies.Any())
        {
            if (matchedStrategies.Length > 1)
            {
                Logging.Log.Info($@"Multiple CRUD implementation Strategies Found, using {matchedStrategies.First().GetType().Name} ({string.Join(", ", matchedStrategies.Skip(1).Select(s => s.GetType().Name))})");
            }
            return matchedStrategies.First();
        }
        return null;
    }

    public static ICrudImplementationStrategy GetMatchedQueryStrategy(CSharpTemplateBase<QueryModel> template, IApplication application)
    {
        var strategies = new ICrudImplementationStrategy[]
        {
            new ODataGetAllImplementationStrategy(template, application),
            new QueryMappingImplementationStrategy(template),

            new GetAllImplementationStrategy(template, application),
            new GetByIdImplementationStrategy(template, application),
            new GetAllPaginationImplementationStrategy(template)
        };

        var matchedStrategies = strategies.Where(strategy => strategy.IsMatch()).ToArray();
        if (matchedStrategies.Any())
        {
            if (matchedStrategies.Length > 1)
            {
                Logging.Log.Info($@"Multiple Query Strategies Found, using {matchedStrategies.First().GetType().Name} ({string.Join(", ", matchedStrategies.Skip(1).Select(s => s.GetType().Name))})");
            }
            return matchedStrategies.First();
        }
        return null;
    }
}