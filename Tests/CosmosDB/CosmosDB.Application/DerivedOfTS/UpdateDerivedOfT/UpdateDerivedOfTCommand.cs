using CosmosDB.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CosmosDB.Application.DerivedOfTS.UpdateDerivedOfT
{
    public class UpdateDerivedOfTCommand : IRequest, ICommand
    {
        public UpdateDerivedOfTCommand(string id, string derivedAttribute, int genericAttribute)
        {
            Id = id;
            DerivedAttribute = derivedAttribute;
            GenericAttribute = genericAttribute;
        }

        public string Id { get; set; }
        public string DerivedAttribute { get; set; }
        public int GenericAttribute { get; set; }
    }
}