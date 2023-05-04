using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Modelers.Services.GraphQL.Api;

namespace Intent.Modules.HotChocolate.GraphQL.Templates.QueryResolver;

public interface IGraphQLMutationTypeModel : IHasName
{
    IEnumerable<IGraphQLResolverModel> Resolvers { get; }
}
public interface IGraphQLQueryTypeModel : IHasName
{
    IEnumerable<IGraphQLResolverModel> Resolvers { get; }
}

public interface IGraphQLResolverModel : IHasName, IHasTypeReference
{
    IEnumerable<IGraphQLParameterModel> Parameters { get; }
    IElementMapping Mapping { get; }
}

public interface IGraphQLParameterModel : IHasName, IHasTypeReference
{
    IElementMapping Mapping { get; }
}