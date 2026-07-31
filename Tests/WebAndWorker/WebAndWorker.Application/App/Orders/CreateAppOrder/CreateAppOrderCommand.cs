using Intent.RoslynWeaver.Attributes;
using MediatR;
using WebAndWorker.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace WebAndWorker.Application.App.Orders.CreateAppOrder
{
    public class CreateAppOrderCommand : IRequest, ICommand
    {
        public CreateAppOrderCommand(string description)
        {
            Description = description;
        }

        public string Description { get; set; }
    }
}