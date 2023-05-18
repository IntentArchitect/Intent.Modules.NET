using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Application.MediatR.Api;
using Intent.Metadata.Models;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Common.Templates;
using Intent.Modules.HotChocolate.GraphQL.Models;

namespace Intent.Modules.HotChocolate.GraphQL.Dispatch.MediatR.ImplicitResolvers;

public class QueryGraphQLResolverModel : IGraphQLResolverModel
{
    public QueryGraphQLResolverModel(QueryModel query)
    {
        Name = query.Name.RemoveSuffix("Query");
        TypeReference = query.TypeReference;
        Parameters = query.Properties.Select(x => new GraphQLParameterModel(
            name: x.Name.ToCamelCase(),
            typeReference: x.TypeReference,
            mappedElement: x.InternalElement,
            mappedPath: new[] { x.Name },
            description: x.Comment));
        MappedElement = query.InternalElement;
        Description = query.Comment;
        RequiresAuthorization = query.HasAuthorize();
        AuthorizationDetails = query.HasAuthorize() ? new AuthorizationModel(query.GetAuthorize().Roles()?.Split(',', StringSplitOptions.RemoveEmptyEntries), query.GetAuthorize().Policy()) : null;
    }
    public string Name { get; }
    public ITypeReference TypeReference { get; }
    public bool RequiresAuthorization { get; }
    public IAuthorizationModel AuthorizationDetails { get; }
    public IEnumerable<IGraphQLParameterModel> Parameters { get; }
    public IElement MappedElement { get; }
    public string Description { get; }
}

public class CommandGraphQLResolverModel : IGraphQLResolverModel
{
    public CommandGraphQLResolverModel(CommandModel command)
    {
        Name = command.Name.RemoveSuffix("Command");
        TypeReference = command.TypeReference;
        Parameters = Array.Empty<IGraphQLParameterModel>();
        MappedElement = command.InternalElement;
        Description = command.Comment;
        RequiresAuthorization = command.HasAuthorize();
        AuthorizationDetails = command.HasAuthorize() ? new AuthorizationModel(command.GetAuthorize().Roles()?.Split(',', StringSplitOptions.RemoveEmptyEntries), command.GetAuthorize().Policy()) : null;
    }
    public string Name { get; }
    public ITypeReference TypeReference { get; }
    public bool RequiresAuthorization { get; }
    public IAuthorizationModel AuthorizationDetails { get; }
    public IEnumerable<IGraphQLParameterModel> Parameters { get; }
    public IElement MappedElement { get; }
    public string Description { get; }
}