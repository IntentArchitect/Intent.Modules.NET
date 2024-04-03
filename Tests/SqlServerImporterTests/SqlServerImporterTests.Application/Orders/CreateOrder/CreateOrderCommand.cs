using System;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using SqlServerImporterTests.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace SqlServerImporterTests.Application.Orders.CreateOrder
{
    public class CreateOrderCommand : IRequest<Guid>, ICommand
    {
        public CreateOrderCommand(Guid customerId, DateTime orderDate, string refNo)
        {
            CustomerId = customerId;
            OrderDate = orderDate;
            RefNo = refNo;
        }

        public Guid CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public string RefNo { get; set; }
    }
}