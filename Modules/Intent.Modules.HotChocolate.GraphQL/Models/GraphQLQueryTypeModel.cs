using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.Templates;
using Intent.Modelers.Services.GraphQL.Api;

namespace Intent.Modules.HotChocolate.GraphQL.Models;

class GraphQLQueryTypeModel : IGraphQLQueryTypeModel
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

class GraphQLResolverModel : IGraphQLResolverModel
{
    public GraphQLResolverModel(GraphQLSchemaFieldModel model)
    {
        Name = model.Name;
        TypeReference = model.TypeReference;
        MappedElement = model.Mapping?.Element as IElement;
        Parameters = model.Parameters.Select(x => new GraphQLParameterModel(x.Name, x.TypeReference, x.InternalElement.MappedElement, x.Comment)).ToList();
        Description = model.Comment;
        RequiresAuthorization = false;
        AuthorizationDetails = null;
    }

    public GraphQLResolverModel(GraphQLMutationModel model)
    {
        Name = model.Name;
        TypeReference = model.TypeReference;
        Description = model.Comment;
        MappedElement = model.Mapping?.Element as IElement;
        Parameters = model.Parameters.Select(x => new GraphQLParameterModel(x.Name, x.TypeReference, x.InternalElement.MappedElement, x.Comment)).ToList();
        Description = model.Comment;
        RequiresAuthorization = false;
        AuthorizationDetails = null;
    }

    public string Name { get; }
    public ITypeReference TypeReference { get; }
    public bool RequiresAuthorization { get; }
    public IAuthorizationModel AuthorizationDetails { get; }
    public IEnumerable<IGraphQLParameterModel> Parameters { get; }
    public IElement MappedElement { get; }
    public string Description { get; }
}

public class GraphQLParameterModel : IGraphQLParameterModel
{
    public GraphQLParameterModel(string name, ITypeReference typeReference, IElement mappedElement, string[] mappedPath, string description)
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
    public IElement MappedElement { get; }
    public string[] MappedPath { get; }
    public string Description { get; }
}

public class AuthorizationModel : IAuthorizationModel
{
    public AuthorizationModel(IEnumerable<string> roles, string policy)
    {
        Roles = roles ?? Array.Empty<string>();
        Policy = policy;
    }
    public IEnumerable<string> Roles { get; }
    public string Policy { get; }
}