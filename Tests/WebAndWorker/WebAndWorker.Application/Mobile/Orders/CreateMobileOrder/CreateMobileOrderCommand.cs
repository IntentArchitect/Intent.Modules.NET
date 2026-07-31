using Intent.RoslynWeaver.Attributes;
using MediatR;
using WebAndWorker.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace WebAndWorker.Application.Mobile.Orders.CreateMobileOrder
{
    public class CreateMobileOrderCommand : IRequest, ICommand
    {
        public CreateMobileOrderCommand(string description)
        {
            Description = description;
        }

        public string Description { get; set; }
    }
}