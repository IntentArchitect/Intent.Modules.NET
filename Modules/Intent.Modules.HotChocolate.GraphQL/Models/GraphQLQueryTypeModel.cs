#nullable enable
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Services.GraphQL.Api;
using Intent.Modules.Common.Templates;
using Intent.Modules.Metadata.Security.Models;

namespace Intent.Modules.HotChocolate.GraphQL.Models;

internal class GraphQLQueryTypeModel : IGraphQLQueryTypeModel
{
    public GraphQLQueryTypeModel(Intent.Modelers.Services.GraphQL.Api.GraphQLQueryTypeModel model)
    {
        Id = model.Id;
        Name = model.Name;
        Resolvers = model.Queries.Select(x => new GraphQLResolverModel(x)).ToList();
    }

    public string Id { get; }
    public string Name { get; }
    public IEnumerable<IGraphQLResolverModel> Resolvers { get; }
}

internal class GraphQLResolverModel : IGraphQLResolverModel
{
    public GraphQLResolverModel(GraphQLSchemaFieldModel model)
    {
        Name = model.Name;
        TypeReference = model.TypeReference;
        MappedElement = model.Mapping?.Element as IElement;
        Parameters = model.Parameters.Select(x => new GraphQLParameterModel(x.Name, x.TypeReference, x.InternalElement.MappedElement, x.Comment)).ToList();
        Description = model.Comment;
        SecurityModels = null;
    }

    public GraphQLResolverModel(GraphQLMutationModel model)
    {
        Name = model.Name;
        TypeReference = model.TypeReference;
        Description = model.Comment;
        MappedElement = model.Mapping?.Element as IElement;
        Parameters = model.Parameters.Select(x => new GraphQLParameterModel(x.Name, x.TypeReference, x.InternalElement.MappedElement, x.Comment)).ToList();
        Description = model.Comment;
        SecurityModels = null;
    }

    public string Name { get; }
    public ITypeReference TypeReference { get; }
    public IReadOnlyCollection<ISecurityModel>? SecurityModels { get; }
    public IEnumerable<IGraphQLParameterModel> Parameters { get; }
    public IElement? MappedElement { get; }
    public string Description { get; }
}

public class GraphQLParameterModel : IGraphQLParameterModel
{
    public GraphQLParameterModel(string name, ITypeReference typeReference, IElement? mappedElement, string[]? mappedPath, string description)
    {
        Name = name;
        TypeReference = typeReference;
        MappedElement = mappedElement;
        MappedPath = mappedPath;
        Description = description;
    }

    public GraphQLParameterModel(string name, ITypeReference typeReference, IElementMapping mapping, string description)
        : this(name, typeReference, mapping?.Element as IElement, mapping?.Path.Select(x => x.Name.ToPascalCase()).ToArray(), description)
    {
    }

    public string Name { get; }
    public ITypeReference TypeReference { get; }
    public IElement? MappedElement { get; }
    public string[]? MappedPath { get; }
    public string Description { get; }
}