using CleanArchitecture.Comprehensive.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.ComponentTypes.GetComponentTypePropertyById
{
    public class GetComponentTypePropertyByIdQuery : IRequest<ComponentTypePropertyDto>, IQuery
    {
        public GetComponentTypePropertyByIdQuery(int componentTypeId, int propertyGroupId, int propertyId)
        {
            ComponentTypeId = componentTypeId;
            PropertyGroupId = propertyGroupId;
            PropertyId = propertyId;
        }

        public int ComponentTypeId { get; set; }
        public int PropertyGroupId { get; set; }
        public int PropertyId { get; set; }
    }
}