using System.Collections.Generic;
using Intent.Metadata.Models;

namespace Intent.Modules.HotChocolate.GraphQL.Models;

public interface IGraphQLResolverModel : IHasName, IHasTypeReference
{
    bool RequiresAuthorization { get; }
    IAuthorizationModel AuthorizationDetails { get; }
    IEnumerable<IGraphQLParameterModel> Parameters { get; }
    IElement MappedElement { get; }
    string Description { get; }
}

public interface IAuthorizationModel
{
    IEnumerable<string> Roles { get; }
    string Policy { get; }
}