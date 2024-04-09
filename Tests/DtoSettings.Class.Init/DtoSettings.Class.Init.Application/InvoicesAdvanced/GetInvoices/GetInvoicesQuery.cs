using System.Collections.Generic;
using DtoSettings.Class.Init.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace DtoSettings.Class.Init.Application.InvoicesAdvanced.GetInvoices
{
    public class GetInvoicesQuery : IRequest<List<InvoiceDto>>, IQuery
    {
        public GetInvoicesQuery()
        {
        }
    }
}