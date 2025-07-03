using AzureFunctions.AzureEventGrid.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AzureFunctions.AzureEventGrid.Application.CreateOrder
{
    public class CreateOrderCommand : IRequest, ICommand
    {
        public CreateOrderCommand(string refNo)
        {
            RefNo = refNo;
        }

        public string RefNo { get; set; }
    }
}