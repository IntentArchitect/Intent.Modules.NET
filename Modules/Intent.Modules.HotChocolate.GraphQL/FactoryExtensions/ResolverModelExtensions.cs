using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Modelers.Services.Api;
using Intent.Modules.HotChocolate.GraphQL.Models;

namespace Intent.Modules.HotChocolate.GraphQL.FactoryExtensions;

public static class ResolverModelExtensions
{
    public static IEnumerable<IGraphQLParameterModel> GetMatchingInDtoParameters(this IGraphQLResolverModel resolver, DTOModel dtoModel)
    {
        return resolver.Parameters.Where(x => dtoModel.Fields.Any(f => f.Name.Equals(x.Name, StringComparison.InvariantCultureIgnoreCase))).ToList();
    }
}