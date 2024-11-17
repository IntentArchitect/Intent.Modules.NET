#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Common.Templates;
using Intent.Modules.HotChocolate.GraphQL.Models;
using Intent.Modules.Metadata.Security.Models;

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
            mappedPath: [x.Name],
            description: x.Comment));
        MappedElement = query.InternalElement;
        Description = query.Comment;
        SecurityModels = SecurityModelHelpers.GetSecurityModels(query.InternalElement).ToArray();
    }
    public string Name { get; }
    public ITypeReference TypeReference { get; }
    public IReadOnlyCollection<ISecurityModel>? SecurityModels { get; }
    public IEnumerable<IGraphQLParameterModel> Parameters { get; }
    public IElement? MappedElement { get; }
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
        SecurityModels = SecurityModelHelpers.GetSecurityModels(command.InternalElement).ToArray();
    }
    public string Name { get; }
    public ITypeReference TypeReference { get; }
    public IReadOnlyCollection<ISecurityModel>? SecurityModels { get; }
    public IEnumerable<IGraphQLParameterModel> Parameters { get; }
    public IElement? MappedElement { get; }
    public string Description { get; }
}