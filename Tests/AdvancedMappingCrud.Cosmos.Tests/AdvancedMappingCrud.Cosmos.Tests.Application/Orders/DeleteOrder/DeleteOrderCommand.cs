using AdvancedMappingCrud.Cosmos.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.Orders.DeleteOrder
{
    public class DeleteOrderCommand : IRequest, ICommand
    {
        public DeleteOrderCommand(string id)
        {
            Id = id;
        }

        public string Id { get; set; }
    }
}