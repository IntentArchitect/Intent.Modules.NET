using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.Application.DomainInteractions.Mapping.Resolvers;

public class LookupIdSynchronizeCollectionMapping : CSharpMappingBase
{
    public LookupIdSynchronizeCollectionMapping(MappingModel model, ICSharpTemplate template) : base(model, template)
    {
    }

    public override CSharpStatement GetSourceStatement(bool? withNullConditionalOperators = null)
    {
        var lookupIdMapping = Children.First().Mapping;
        var from = $"{Template.GetTypeName("Domain.Common.UpdateHelper")}.SynchronizeCollection({GetTargetPathText()}, {GetSourcePathText(lookupIdMapping.TargetPath, false)}, (e, d) => {GetPrimaryKeyComparisonMappings()})";
        return from;
    }
    
    private string GetPrimaryKeyComparisonMappings()
    {
        var idAttrElement = (Model.TypeReference.Element as IElement)?.ChildElements?.FirstOrDefault(x => x.HasStereotype("Primary Key"));
        
        if (idAttrElement is null)
        {
            return "false";
        }

        return $"e.{idAttrElement.Name} == d.{idAttrElement.Name}";
    }
}