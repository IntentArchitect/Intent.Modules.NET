using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using TableStorage.Tests.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace TableStorage.Tests.Application.Orders.UpdateOrder
{
    public class UpdateOrderCommand : IRequest, ICommand
    {
        public UpdateOrderCommand(string partitionKey,
            string rowKey,
            string orderNo,
            decimal amount,
            UpdateOrderCustomerDto customer)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
            OrderNo = orderNo;
            Amount = amount;
            Customer = customer;
        }

        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public string OrderNo { get; set; }
        public decimal Amount { get; set; }
        public UpdateOrderCustomerDto Customer { get; set; }
    }
}