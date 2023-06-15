using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.ServiceContract", Version = "1.0")]

namespace Standard.AspNetCore.TestApplication.Application.IntegrationServices.IntegrationServiceProxy
{
    public interface IIntegrationServiceProxyClient : IDisposable
    {
        Task<CustomDTO> QueryParamOpAsync(string param1, int param2, CancellationToken cancellationToken = default);
        Task HeaderParamOpAsync(string param1, CancellationToken cancellationToken = default);
        Task FormParamOpAsync(string param1, int param2, CancellationToken cancellationToken = default);
        Task RouteParamOpAsync(string param1, CancellationToken cancellationToken = default);
        Task BodyParamOpAsync(CustomDTO param1, CancellationToken cancellationToken = default);
        Task ThrowsExceptionAsync(CancellationToken cancellationToken = default);
        Task<Guid> GetWrappedPrimitiveGuidAsync(CancellationToken cancellationToken = default);
        Task<string> GetWrappedPrimitiveStringAsync(CancellationToken cancellationToken = default);
        Task<int> GetWrappedPrimitiveIntAsync(CancellationToken cancellationToken = default);
        Task<Guid> GetPrimitiveGuidAsync(CancellationToken cancellationToken = default);
        Task<string> GetPrimitiveStringAsync(CancellationToken cancellationToken = default);
        Task<int> GetPrimitiveIntAsync(CancellationToken cancellationToken = default);
        Task<List<string>> GetPrimitiveStringListAsync(CancellationToken cancellationToken = default);
        Task<CustomDTO> GetInvoiceOpWithReturnTypeWrappedAsync(CancellationToken cancellationToken = default);
    }
}