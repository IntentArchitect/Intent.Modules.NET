#nullable enable
using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modules.Metadata.Security.Models;

namespace Intent.Modules.HotChocolate.GraphQL.Models;

public interface IGraphQLResolverModel : IHasName, IHasTypeReference
{
    IReadOnlyCollection<ISecurityModel>? SecurityModels { get; }
    IEnumerable<IGraphQLParameterModel> Parameters { get; }
    IElement? MappedElement { get; }
    string Description { get; }
}

public interface IAuthorizationModel
{
    IEnumerable<string> Roles { get; }
    string Policy { get; }
}