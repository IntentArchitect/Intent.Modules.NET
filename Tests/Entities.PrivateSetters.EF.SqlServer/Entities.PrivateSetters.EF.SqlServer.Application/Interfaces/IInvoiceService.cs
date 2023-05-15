using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace Entities.PrivateSetters.EF.SqlServer.Application.Interfaces
{

    public interface IInvoiceService : IDisposable
    {
        Task Create(CreateInvoiceDto dto);
        Task<List<InvoiceDto>> GetAll();

    }
}