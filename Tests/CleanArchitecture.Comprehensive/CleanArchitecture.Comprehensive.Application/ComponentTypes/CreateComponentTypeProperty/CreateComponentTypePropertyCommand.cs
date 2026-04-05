using CleanArchitecture.Comprehensive.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.ComponentTypes.CreateComponentTypeProperty
{
    public class CreateComponentTypePropertyCommand : IRequest<int>, ICommand
    {
        public CreateComponentTypePropertyCommand(int componentTypeId, int propertyGroupId, string propertyName)
        {
            ComponentTypeId = componentTypeId;
            PropertyGroupId = propertyGroupId;
            PropertyName = propertyName;
        }

        public int ComponentTypeId { get; set; }
        public int PropertyGroupId { get; set; }
        public string PropertyName { get; set; }
    }
}