using System.Collections.Generic;
using Intent.Modules.HotChocolate.GraphQL.Templates.QueryResolver;

namespace Intent.Modules.HotChocolate.GraphQL.Dispatch.MediatR.ImplicitResolvers;

public class MediatRGraphQLQueryTypeModel : IGraphQLQueryTypeModel
{
    public MediatRGraphQLQueryTypeModel(string name, IEnumerable<IGraphQLResolverModel> resolvers)
    {
        Name = name;
        Resolvers = resolvers;
    }

    public string Name { get; }
    public IEnumerable<IGraphQLResolverModel> Resolvers { get; }
}

public class MediatRGraphQLMutationTypeModel : IGraphQLMutationTypeModel
{
    public MediatRGraphQLMutationTypeModel(string name, IEnumerable<IGraphQLResolverModel> resolvers)
    {
        Name = name;
        Resolvers = resolvers;
    }

    public string Name { get; }
    public IEnumerable<IGraphQLResolverModel> Resolvers { get; }
}