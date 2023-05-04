using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Modelers.Services.GraphQL.Api;

namespace Intent.Modules.HotChocolate.GraphQL.Templates.QueryResolver;

class GraphQLMutationTypeModel : IGraphQLMutationTypeModel
{
    public GraphQLMutationTypeModel(Modelers.Services.GraphQL.Api.GraphQLMutationTypeModel model)
    {
        Name = model.Name;
        Resolvers = model.Mutations.Select(x => new GraphQLResolverModel(x)).ToList();
    }

    public string Name { get; }
    public IEnumerable<IGraphQLResolverModel> Resolvers { get; }
}
class GraphQLQueryTypeModel : IGraphQLQueryTypeModel
{
    public GraphQLQueryTypeModel(Modelers.Services.GraphQL.Api.GraphQLQueryTypeModel model)
    {
        Name = model.Name;
        Resolvers = model.Queries.Select(x => new GraphQLResolverModel(x)).ToList();
    }

    public string Name { get; }
    public IEnumerable<IGraphQLResolverModel> Resolvers { get; }
}

class GraphQLResolverModel : IGraphQLResolverModel
{
    public GraphQLResolverModel(GraphQLSchemaFieldModel model)
    {
        Name = model.Name;
        TypeReference = model.TypeReference;
        Mapping = model.Mapping;
        Parameters = model.Parameters.Select(x => new GraphQLParameterModel(x.Name, x.TypeReference, x.InternalElement.MappedElement)).ToList();
    }

    public GraphQLResolverModel(GraphQLMutationModel model)
    {
        Name = model.Name;
        TypeReference = model.TypeReference;
        Mapping = model.Mapping;
        Parameters = model.Parameters.Select(x => new GraphQLParameterModel(x.Name, x.TypeReference, x.InternalElement.MappedElement)).ToList();
    }

    public string Name { get; }
    public ITypeReference TypeReference { get; }
    public IElementMapping Mapping { get; }
    public IEnumerable<IGraphQLParameterModel> Parameters { get; }
}

class GraphQLParameterModel : IGraphQLParameterModel
{
    public GraphQLParameterModel(string name, ITypeReference typeReference, IElementMapping mapping)
    {
        Name = name;
        TypeReference = typeReference;
        Mapping = mapping;
    }

    public string Name { get; }
    public ITypeReference TypeReference { get; }
    public IElementMapping Mapping { get; }
}