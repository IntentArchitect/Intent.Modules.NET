using Intent.Metadata.Models;

namespace Intent.Modules.HotChocolate.GraphQL.Models;

public interface IGraphQLParameterModel : IHasName, IHasTypeReference
{
    IElement MappedElement { get; }
    string[] MappedPath { get; }
    string Description { get; }
}