using AdvancedMapping.Repositories.Mapperly.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.Orders.CreateOrder
{
    public class CreateOrderCommand : IRequest<Guid>, ICommand
    {
        public CreateOrderCommand(Guid customerId, DateTime orderDate, DateTime? requiredBy, string status, decimal totalAmount)
        {
            CustomerId = customerId;
            OrderDate = orderDate;
            RequiredBy = requiredBy;
            Status = status;
            TotalAmount = totalAmount;
        }

        public Guid CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? RequiredBy { get; set; }
        public string Status { get; set; }
        public decimal TotalAmount { get; set; }
    }
}