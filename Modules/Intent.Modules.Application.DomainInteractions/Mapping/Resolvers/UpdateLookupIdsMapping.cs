using System.Linq;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.Application.DomainInteractions.Mapping.Resolvers;

// We have a very special case where mapping your surrogate IDs to a list of existing entities containing a Lookup IDs
// static mapping element will trigger another repository to be loaded and fetching those entities by their IDs and then
// passing them to whoever references the variable name returned by the GetSourceStatement().
internal class UpdateLookupIdsMapping : CSharpMappingBase
{
    public UpdateLookupIdsMapping(MappingModel model, ICSharpTemplate template) : base(model, template)
    {
    }

    public override CSharpStatement GetSourceStatement(bool? withNullConditionalOperators = null)
    {
        if (!Children.Any())
        {
            return base.GetSourceStatement(withNullConditionalOperators);
        }
        var lookupIdMapping = Children.First().Mapping;
        // The Lookup ID mapping target path is associated with the variable name needed for getting the list of existing entities.
        return GetSourcePathText(lookupIdMapping.TargetPath, false);
    }
}