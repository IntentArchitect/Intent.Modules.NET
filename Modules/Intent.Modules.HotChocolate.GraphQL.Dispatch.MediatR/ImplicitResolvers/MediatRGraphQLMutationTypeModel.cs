using System.Collections.Generic;
using Intent.Modules.HotChocolate.GraphQL.Models;

namespace Intent.Modules.HotChocolate.GraphQL.Dispatch.MediatR.ImplicitResolvers;

public class MediatRGraphQLMutationTypeModel : IGraphQLMutationTypeModel
{
    public MediatRGraphQLMutationTypeModel(string id, string name, IEnumerable<IGraphQLResolverModel> resolvers)
    {
        Id = id;
        Name = name;
        Resolvers = resolvers;
    }

    public string Id { get; }
    public string Name { get; }
    public IEnumerable<IGraphQLResolverModel> Resolvers { get; }
}