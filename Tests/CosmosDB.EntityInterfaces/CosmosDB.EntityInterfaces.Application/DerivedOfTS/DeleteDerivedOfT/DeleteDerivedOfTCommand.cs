using CosmosDB.EntityInterfaces.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CosmosDB.EntityInterfaces.Application.DerivedOfTS.DeleteDerivedOfT
{
    public class DeleteDerivedOfTCommand : IRequest, ICommand
    {
        public DeleteDerivedOfTCommand(string id)
        {
            Id = id;
        }

        public string Id { get; set; }
    }
}