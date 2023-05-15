using System.Collections.Generic;
using Intent.Metadata.Models;

namespace Intent.Modules.HotChocolate.GraphQL.Models;

public interface IGraphQLResolverModel : IHasName, IHasTypeReference
{
    IEnumerable<IGraphQLParameterModel> Parameters { get; }
    IElement MappedElement { get; }
    string Description { get; }
}