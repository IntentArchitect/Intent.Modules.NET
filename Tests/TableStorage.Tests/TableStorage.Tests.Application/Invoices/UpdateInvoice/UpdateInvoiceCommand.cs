using System;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using TableStorage.Tests.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace TableStorage.Tests.Application.Invoices.UpdateInvoice
{
    public class UpdateInvoiceCommand : IRequest, ICommand
    {
        public UpdateInvoiceCommand(string partitionKey,
            string rowKey,
            DateTime issuedData,
            string orderPartitionKey,
            string orderRowKey)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
            IssuedData = issuedData;
            OrderPartitionKey = orderPartitionKey;
            OrderRowKey = orderRowKey;
        }

        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTime IssuedData { get; set; }
        public string OrderPartitionKey { get; set; }
        public string OrderRowKey { get; set; }
    }
}