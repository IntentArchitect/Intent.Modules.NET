using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Modelers.Services.GraphQL.Api;

namespace Intent.Modules.HotChocolate.GraphQL.Templates.QueryType;

public interface IGraphQLMutationTypeModel : IMetadataModel, IHasName
{
    IEnumerable<IGraphQLResolverModel> Resolvers { get; }
}
public interface IGraphQLQueryTypeModel : IMetadataModel, IHasName
{
    IEnumerable<IGraphQLResolverModel> Resolvers { get; }
}

public interface IGraphQLResolverModel : IHasName, IHasTypeReference
{
    IEnumerable<IGraphQLParameterModel> Parameters { get; }
    IElement MappedElement { get; }
    string Description { get; }
}

public interface IGraphQLParameterModel : IHasName, IHasTypeReference
{
    IElement MappedElement { get; }
    string[] MappedPath { get; }
    string Description { get; }
}