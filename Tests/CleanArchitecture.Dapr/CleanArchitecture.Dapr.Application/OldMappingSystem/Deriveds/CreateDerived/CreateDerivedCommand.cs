using CleanArchitecture.Dapr.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.Dapr.Application.OldMappingSystem.Deriveds.CreateDerived
{
    public class CreateDerivedCommand : IRequest<string>, ICommand
    {
        public CreateDerivedCommand(string attribute, string baseAttribute)
        {
            Attribute = attribute;
            BaseAttribute = baseAttribute;
        }

        public string Attribute { get; set; }
        public string BaseAttribute { get; set; }
    }
}