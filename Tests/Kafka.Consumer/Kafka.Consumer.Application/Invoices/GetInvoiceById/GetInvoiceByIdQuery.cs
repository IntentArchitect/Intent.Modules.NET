using System;
using Intent.RoslynWeaver.Attributes;
using Kafka.Consumer.Application.Common.Interfaces;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace Kafka.Consumer.Application.Invoices.GetInvoiceById
{
    public class GetInvoiceByIdQuery : IRequest<InvoiceDto>, IQuery
    {
        public GetInvoiceByIdQuery(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}