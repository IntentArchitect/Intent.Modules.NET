using System.Collections.Generic;
using Intent.Modules.HotChocolate.GraphQL.Templates.QueryResolver;

namespace Intent.Modules.HotChocolate.GraphQL.Dispatch.MediatR.ImplicitResolvers;

public class ServiceGraphQLQueryTypeModel : IGraphQLQueryTypeModel
{
    public ServiceGraphQLQueryTypeModel(string name, IEnumerable<IGraphQLResolverModel> resolvers)
    {
        Name = name;
        Resolvers = resolvers;
    }

    public string Name { get; }
    public IEnumerable<IGraphQLResolverModel> Resolvers { get; }
}