using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using TableStorage.Tests.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace TableStorage.Tests.Application.Orders.CreateOrder
{
    public class CreateOrderCommand : IRequest, ICommand
    {
        public CreateOrderCommand(string partitionKey,
            string rowKey,
            string orderNo,
            decimal amount,
            CreateOrderCustomerDto customer,
            List<CreateOrderOrderLineDto> orderLines)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
            OrderNo = orderNo;
            Amount = amount;
            Customer = customer;
            OrderLines = orderLines;
        }

        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public string OrderNo { get; set; }
        public decimal Amount { get; set; }
        public CreateOrderCustomerDto Customer { get; set; }
        public List<CreateOrderOrderLineDto> OrderLines { get; set; }
    }
}