using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.DomainInteractions.Api;
using Intent.Modules.Common.CSharp.Builder;

namespace Intent.Modules.Application.DomainInteractions;

internal class EndPointContext
{
    private readonly CSharpClass _handlerClass;
    private readonly ClassModel _entity;
    private readonly QueryEntityActionTargetEndModel _queryAction;

    public EndPointContext(CSharpClass handlerClass, QueryEntityActionTargetEndModel queryAction)
    {
        _handlerClass = handlerClass;
        _queryAction = queryAction;
    }
}
