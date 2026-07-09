using Intent.RoslynWeaver.Attributes;
using Wolverine.AspNetCore.Controllers.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.CommandModels", Version = "1.0")]

namespace Wolverine.AspNetCore.Controllers.Application.PlaceOrder
{
    public class PlaceOrderCommand : ICommand
    {
        public PlaceOrderCommand(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}