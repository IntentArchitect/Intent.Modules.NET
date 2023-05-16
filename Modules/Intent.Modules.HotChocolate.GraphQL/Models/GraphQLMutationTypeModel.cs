using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.HotChocolate.GraphQL.Models;

class GraphQLMutationTypeModel : IGraphQLMutationTypeModel
{
    public GraphQLMutationTypeModel(Intent.Modelers.Services.GraphQL.Api.GraphQLMutationTypeModel model)
    {
        Id = model.Id;
        Name = model.Name;
        Resolvers = model.Mutations.Select(x => new GraphQLResolverModel(x)).ToList();
    }
    public string Id { get; }
    public string Name { get; }
    public IEnumerable<IGraphQLResolverModel> Resolvers { get; }
}