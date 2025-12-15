using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Interactions;
using Intent.Modules.Common.CSharp.Mapping;

namespace Intent.Modules.Application.DomainInteractions.DataAccessProviders;

/// <summary>
/// Extension methods for IDataAccessProvider to support Lookup IDs mappings (surrogate IDs to entity collections).
/// Each provider implements this as a virtual method that can be overridden.
/// </summary>
public static class IDataAccessProviderExtensions
{
    public static void ProcessLookupIdsMappings(
        this IDataAccessProvider provider,
        ICSharpClassMethodDeclaration method,
        IElementToElementMapping mapping,
        CSharpClassMappingManager csharpMapping,
        List<CSharpStatement> statements)
    {
        // Dispatch to the actual implementation based on provider type
        if (provider is RepositoryDataAccessProvider repositoryProvider)
        {
            repositoryProvider.ProcessLookupIdsMappingsImpl(method, mapping, csharpMapping, statements);
        }
        else if (provider is DbContextDataAccessProvider dbContextProvider)
        {
            dbContextProvider.ProcessLookupIdsMappingsImpl(method, mapping, csharpMapping, statements);
        }
        else if (provider is CompositeDataAccessProvider compositeProvider)
        {
            compositeProvider.ProcessLookupIdsMappingsImpl(method, mapping, csharpMapping, statements);
        }
    }
}
