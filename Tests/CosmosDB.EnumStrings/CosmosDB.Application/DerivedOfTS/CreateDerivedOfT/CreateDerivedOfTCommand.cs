using CosmosDB.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CosmosDB.Application.DerivedOfTS.CreateDerivedOfT
{
    public class CreateDerivedOfTCommand : IRequest<string>, ICommand
    {
        public CreateDerivedOfTCommand(string derivedAttribute, int genericAttribute)
        {
            DerivedAttribute = derivedAttribute;
            GenericAttribute = genericAttribute;
        }

        public string DerivedAttribute { get; set; }
        public int GenericAttribute { get; set; }
    }
}