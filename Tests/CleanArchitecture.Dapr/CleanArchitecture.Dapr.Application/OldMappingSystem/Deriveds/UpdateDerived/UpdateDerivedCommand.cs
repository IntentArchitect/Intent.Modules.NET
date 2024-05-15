using CleanArchitecture.Dapr.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.Dapr.Application.OldMappingSystem.Deriveds.UpdateDerived
{
    public class UpdateDerivedCommand : IRequest, ICommand
    {
        public UpdateDerivedCommand(string id, string attribute, string baseAttribute)
        {
            Id = id;
            Attribute = attribute;
            BaseAttribute = baseAttribute;
        }

        public string Id { get; set; }
        public string Attribute { get; set; }
        public string BaseAttribute { get; set; }
    }
}