using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;

namespace Intent.Modules.HotChocolate.GraphQL.Models;

public interface IGraphQLQueryTypeModel : IMetadataModel, IHasName
{
    IEnumerable<IGraphQLResolverModel> Resolvers { get; }
}