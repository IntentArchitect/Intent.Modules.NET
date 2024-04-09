using System;
using DtoSettings.Class.Private.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace DtoSettings.Class.Private.Application.InvoicesAdvanced.GetInvoiceById
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