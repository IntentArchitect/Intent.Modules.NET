using System;
using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Interactions;
using Intent.Modules.Common.CSharp.Mapping;

namespace Intent.Modules.Application.DomainInteractions.DataAccessProviders;

/// Before we promote this to the IDataAccessProvider interface, let's finalize the design.
internal static class IDataAccessProviderExtensions
{
    public static IList<CSharpStatement> GetAggregateEntityRetrievalStatements(
        this IDataAccessProvider provider,
        ICSharpClassMethodDeclaration method,
        IElementToElementMapping mapping,
        CSharpClassMappingManager csharpMapping)
    {
        return provider switch
        {
            RepositoryDataAccessProvider repositoryProvider => 
                repositoryProvider.GetAggregateEntityRetrievalStatements(method, mapping, csharpMapping),
            DbContextDataAccessProvider dbContextProvider =>
                dbContextProvider.GetAggregateEntityRetrievalStatements(method, mapping, csharpMapping),
            CompositeDataAccessProvider compositeProvider =>
                compositeProvider.GetAggregateEntityRetrievalStatements(method, mapping, csharpMapping),
            _ => throw new NotSupportedException("The IDataAccessProvider type is not supported for GetAggregateEntityRetrievalStatements.")
        };
    }
}
