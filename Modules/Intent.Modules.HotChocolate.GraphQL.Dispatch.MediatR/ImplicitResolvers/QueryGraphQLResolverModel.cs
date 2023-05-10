using System;
using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Common.Templates;
using Intent.Modules.HotChocolate.GraphQL.Templates.QueryResolver;

namespace Intent.Modules.HotChocolate.GraphQL.Dispatch.MediatR.ImplicitResolvers;

public class QueryGraphQLResolverModel : IGraphQLResolverModel
{
    public QueryGraphQLResolverModel(QueryModel query)
    {
        Name = query.Name.RemoveSuffix("Query");
        TypeReference = query.TypeReference;
        Parameters = Array.Empty<IGraphQLParameterModel>();
        MappedElement = query.InternalElement;
    }
    public string Name { get; }
    public ITypeReference TypeReference { get; }
    public IEnumerable<IGraphQLParameterModel> Parameters { get; }
    public IElement MappedElement { get; }
}

public class CommandGraphQLResolverModel : IGraphQLResolverModel
{
    public CommandGraphQLResolverModel(CommandModel command)
    {
        Name = command.Name.RemoveSuffix("Command");
        TypeReference = command.TypeReference;
        Parameters = Array.Empty<IGraphQLParameterModel>();
        MappedElement = command.InternalElement;
    }
    public string Name { get; }
    public ITypeReference TypeReference { get; }
    public IEnumerable<IGraphQLParameterModel> Parameters { get; }
    public IElement MappedElement { get; }
}