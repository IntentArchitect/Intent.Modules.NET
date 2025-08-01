using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.Blazor.Api.Mappings;

public class CallServiceOperationMappingResolver : IMappingTypeResolver
{
    private readonly ICSharpTemplate _template;

    public CallServiceOperationMappingResolver(ICSharpTemplate template)
    {
        _template = template;
    }

    public ICSharpMapping? ResolveMappings(MappingModel mappingModel)
    {

        if (mappingModel.Model.SpecializationType is "Operation" or "Component Operation")
        {
            return new MethodInvocationMapping(mappingModel, _template);
        }

        if (mappingModel.Mapping?.MappingTypeId == "720f119b-39b3-4f11-8d96-27fa82d1f4e2" // Invocation Mapping
            && mappingModel.Model.SpecializationType is "Event Emitter")
        {
            return new RazorEventEmitterInvocationMapping(mappingModel, _template);
        }

        const string httpSettingsDefinitionId = "b4581ed2-42ec-4ae2-83dd-dcdd5f0837b6";
        const string dtoFieldTypeId = "7baed1fd-469b-4980-8fd9-4cefb8331eb2";
        if (mappingModel.Model.SpecializationType is "Command" or "Query" &&
            ((IElement)mappingModel.Model).HasStereotype(httpSettingsDefinitionId))
        {
            return ((IElement)mappingModel.Model).ChildElements.Count(x => x.SpecializationTypeId is dtoFieldTypeId) == 1
                ? new SingleFieldMapping(mappingModel, _template)
                : new ObjectInitializationMapping(mappingModel, _template);
        }
        
        if (//mappingModel.Model.SpecializationType is "Parameter" && 
            mappingModel.Model.TypeReference?.Element?.SpecializationType is "Command" or "DTO" or "Model Definition" &&
            mappingModel.Model.SpecializationType is not "Event Emitter")
        {
            return new ObjectInitializationMapping(mappingModel, _template);
        }

        return null;
    }

    private class SingleFieldMapping(MappingModel model, ICSharpTemplate template) : CSharpMappingBase(model, template)
    {
        public override CSharpStatement GetSourceStatement(bool? targetIsNullable = null)
        {
            var child = Children.First();
            return child.GetSourceStatement();
        }
    }
}