using AutoMapper;
using CleanArchitecture.Comprehensive.Application.Common.Mappings;
using CleanArchitecture.Comprehensive.Domain.Entities.PrimaryKeyLookup;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.ComponentTypes
{
    public class ComponentTypePropertyDto : IMapFrom<ComponentTypeProperty>
    {
        public ComponentTypePropertyDto()
        {
            PropertyName = null!;
        }

        public int PropertyId { get; set; }
        public string PropertyName { get; set; }
        public int ComponentPropertyGroupPropertyGroupId { get; set; }

        public static ComponentTypePropertyDto Create(
            int propertyId,
            string propertyName,
            int componentPropertyGroupPropertyGroupId)
        {
            return new ComponentTypePropertyDto
            {
                PropertyId = propertyId,
                PropertyName = propertyName,
                ComponentPropertyGroupPropertyGroupId = componentPropertyGroupPropertyGroupId
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<ComponentTypeProperty, ComponentTypePropertyDto>();
        }
    }
}