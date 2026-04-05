using System.Collections.Generic;
using CleanArchitecture.Comprehensive.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.ComponentTypes.GetComponentTypeProperties
{
    public class GetComponentTypePropertiesQuery : IRequest<List<ComponentTypePropertyDto>>, IQuery
    {
        public GetComponentTypePropertiesQuery(int componentTypeId, int propertyGroupId)
        {
            ComponentTypeId = componentTypeId;
            PropertyGroupId = propertyGroupId;
        }

        public int ComponentTypeId { get; set; }
        public int PropertyGroupId { get; set; }
    }
}