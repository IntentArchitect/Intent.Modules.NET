using System.Collections.Generic;
using Intent.Modules.HotChocolate.GraphQL.Templates.QueryResolver;

namespace Intent.Modules.HotChocolate.GraphQL.Dispatch.MediatR.ImplicitResolvers;

public class ServiceGraphQLMutationTypeModel : IGraphQLMutationTypeModel
{
    public ServiceGraphQLMutationTypeModel(string name, IEnumerable<IGraphQLResolverModel> resolvers)
    {
        Name = name;
        Resolvers = resolvers;
    }

    public string Name { get; }
    public IEnumerable<IGraphQLResolverModel> Resolvers { get; }
}