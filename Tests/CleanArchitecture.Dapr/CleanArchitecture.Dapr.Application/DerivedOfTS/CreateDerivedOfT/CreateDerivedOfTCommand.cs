using CleanArchitecture.Dapr.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.Dapr.Application.DerivedOfTS.CreateDerivedOfT
{
    public class CreateDerivedOfTCommand : IRequest<string>, ICommand
    {
        public CreateDerivedOfTCommand(string derivedAttribute, int genericTypeAttribute)
        {
            DerivedAttribute = derivedAttribute;
            GenericTypeAttribute = genericTypeAttribute;
        }

        public string DerivedAttribute { get; set; }
        public int GenericTypeAttribute { get; set; }
    }
}