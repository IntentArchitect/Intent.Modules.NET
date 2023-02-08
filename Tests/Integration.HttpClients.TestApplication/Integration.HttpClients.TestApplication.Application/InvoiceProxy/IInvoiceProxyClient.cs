using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.ServiceContract", Version = "1.0")]

namespace Integration.HttpClients.TestApplication.Application.InvoiceProxy
{
    public interface IInvoiceProxyClient : IDisposable
    {
        Task CreateAsync(InvoiceCreateDTO dto, CancellationToken cancellationToken = default);
        Task<InvoiceDTO> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<InvoiceDTO>> FindAllAsync(CancellationToken cancellationToken = default);
        Task UpdateAsync(Guid id, InvoiceUpdateDTO dto, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task<InvoiceDTO> QueryParamOpAsync(string param1, int param2, CancellationToken cancellationToken = default);
        Task HeaderParamOpAsync(string param1, CancellationToken cancellationToken = default);
        Task FormParamOpAsync(string param1, int param2, CancellationToken cancellationToken = default);
        Task RouteParamOpAsync(string param1, CancellationToken cancellationToken = default);
        Task BodyParamOpAsync(InvoiceDTO param1, CancellationToken cancellationToken = default);
        Task ThrowsExceptionAsync(CancellationToken cancellationToken = default);
        Task<Guid> GetWrappedPrimitiveGuidAsync(CancellationToken cancellationToken = default);
        Task<string> GetWrappedPrimitiveStringAsync(CancellationToken cancellationToken = default);
        Task<int> GetWrappedPrimitiveIntAsync(CancellationToken cancellationToken = default);
        Task<Guid> GetPrimitiveGuidAsync(CancellationToken cancellationToken = default);
        Task<string> GetPrimitiveStringAsync(CancellationToken cancellationToken = default);
        Task<int> GetPrimitiveIntAsync(CancellationToken cancellationToken = default);
        Task<List<string>> GetPrimitiveStringListAsync(CancellationToken cancellationToken = default);
        Task<InvoiceDTO> GetInvoiceOpWithReturnTypeWrappedAsync(CancellationToken cancellationToken = default);
    }
}