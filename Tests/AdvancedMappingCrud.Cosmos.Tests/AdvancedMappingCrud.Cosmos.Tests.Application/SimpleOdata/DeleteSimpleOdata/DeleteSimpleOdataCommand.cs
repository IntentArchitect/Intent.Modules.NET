using AdvancedMappingCrud.Cosmos.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.SimpleOdata.DeleteSimpleOdata
{
    public class DeleteSimpleOdataCommand : IRequest, ICommand
    {
        public DeleteSimpleOdataCommand(string id)
        {
            Id = id;
        }

        public string Id { get; set; }
    }
}