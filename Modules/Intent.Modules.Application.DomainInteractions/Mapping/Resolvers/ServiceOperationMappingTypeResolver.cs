using System.Linq;
using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modelers.Domain.Api;

namespace Intent.Modules.Application.DomainInteractions.Mapping.Resolvers
{
    public class ServiceOperationMappingTypeResolver : IMappingTypeResolver
    {
        private readonly ICSharpFileBuilderTemplate _template;

        public ServiceOperationMappingTypeResolver(ICSharpFileBuilderTemplate template)
        {
            _template = template;
        }

        public ICSharpMapping ResolveMappings(MappingModel mappingModel)
        {
            var model = mappingModel.Model;
            
            if (mappingModel.MappingTypeId != "df692ffe-5d0c-40ee-9362-a483d929a8ec") // Service Operation Mapping
            {
                return null;
            }
            if (model.IsGeneralizationTargetEndModel())
            {
                var mapping = new InheritedChildrenMapping(mappingModel, _template);
                return mapping;
            }

            if (model.SpecializationType == "Stored Procedure")
            {
                return new MethodInvocationMapping(mappingModel, _template);
            }

            if (model.SpecializationType == "Stored Procedure Parameter" &&
                model.TypeReference.Element?.SpecializationType is "Data Contract" or "Class")
            {
                return new ObjectInitializationMapping(mappingModel, _template);
            }

            if (model.SpecializationType == "Domain Service")
            {
                return new MethodInvocationMapping(mappingModel, _template);
            }

            if (model.SpecializationType == "Parameter" &&
                model.TypeReference.Element?.SpecializationType is "Class")
            {                
                return new ObjectInitializationMapping(mappingModel, _template);
            }

            if (model.SpecializationType == "DTO-Field" && mappingModel.Children.Any())
            {
                return new ObjectInitializationMapping(mappingModel, _template);
            }

			return null;
        }
    }
}