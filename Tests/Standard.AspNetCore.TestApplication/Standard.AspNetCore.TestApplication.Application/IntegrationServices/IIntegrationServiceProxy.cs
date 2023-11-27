using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Standard.AspNetCore.TestApplication.Application.IntegrationServices.Services.Integration;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.ServiceContract", Version = "2.0")]

namespace Standard.AspNetCore.TestApplication.Application.IntegrationServices
{
    public interface IIntegrationServiceProxy : IDisposable
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
        Task<List<CustomDTO>> GetItemsAsync(List<string> ids, CancellationToken cancellationToken = default);
    }
}