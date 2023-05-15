using System.Collections.Generic;
using Intent.Metadata.Models;

namespace Intent.Modules.HotChocolate.GraphQL.Models;

public interface IGraphQLMutationTypeModel : IMetadataModel, IHasName
{
    IEnumerable<IGraphQLResolverModel> Resolvers { get; }
}