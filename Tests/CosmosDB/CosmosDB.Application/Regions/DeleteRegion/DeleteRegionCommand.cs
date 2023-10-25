using CosmosDB.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CosmosDB.Application.Regions.DeleteRegion
{
    public class DeleteRegionCommand : IRequest, ICommand
    {
        public DeleteRegionCommand(string id)
        {
            Id = id;
        }

        public string Id { get; set; }
    }
}