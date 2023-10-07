using CleanArchitecture.Dapr.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.Dapr.Application.DerivedOfTS.UpdateDerivedOfT
{
    public class UpdateDerivedOfTCommand : IRequest, ICommand
    {
        public UpdateDerivedOfTCommand(string id, string derivedAttribute, int genericTypeAttribute)
        {
            Id = id;
            DerivedAttribute = derivedAttribute;
            GenericTypeAttribute = genericTypeAttribute;
        }

        public string Id { get; private set; }
        public string DerivedAttribute { get; set; }
        public int GenericTypeAttribute { get; set; }

        public void SetId(string id)
        {
            Id = id;
        }
    }
}