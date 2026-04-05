using CleanArchitecture.Comprehensive.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.ComponentTypes.UpdateComponentTypeProperty
{
    public class UpdateComponentTypePropertyCommand : IRequest, ICommand
    {
        public UpdateComponentTypePropertyCommand(int componentTypeId,
            int propertyGroupId,
            int propertyId,
            string propertyName)
        {
            ComponentTypeId = componentTypeId;
            PropertyGroupId = propertyGroupId;
            PropertyId = propertyId;
            PropertyName = propertyName;
        }

        public int ComponentTypeId { get; set; }
        public int PropertyGroupId { get; set; }
        public int PropertyId { get; set; }
        public string PropertyName { get; set; }
    }
}