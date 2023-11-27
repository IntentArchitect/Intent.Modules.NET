using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Standard.AspNetCore.TestApplication.Application.Integration;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace Standard.AspNetCore.TestApplication.Application.Interfaces
{
    public interface IIntegrationService : IDisposable
    {
        Task<CustomDTO> QueryParamOp(string param1, int param2, CancellationToken cancellationToken = default);
        Task HeaderParamOp(string param1, CancellationToken cancellationToken = default);
        Task FormParamOp(string param1, int param2, CancellationToken cancellationToken = default);
        Task RouteParamOp(string param1, CancellationToken cancellationToken = default);
        Task BodyParamOp(CustomDTO param1, CancellationToken cancellationToken = default);
        Task ThrowsException(CancellationToken cancellationToken = default);
        Task<Guid> GetWrappedPrimitiveGuid(CancellationToken cancellationToken = default);
        Task<string> GetWrappedPrimitiveString(CancellationToken cancellationToken = default);
        Task<int> GetWrappedPrimitiveInt(CancellationToken cancellationToken = default);
        Task<Guid> GetPrimitiveGuid(CancellationToken cancellationToken = default);
        Task<string> GetPrimitiveString(CancellationToken cancellationToken = default);
        Task<int> GetPrimitiveInt(CancellationToken cancellationToken = default);
        Task<List<string>> GetPrimitiveStringList(CancellationToken cancellationToken = default);
        Task NonHttpSettingsOperation(CancellationToken cancellationToken = default);
        Task<CustomDTO> GetInvoiceOpWithReturnTypeWrapped(CancellationToken cancellationToken = default);
        Task<List<CustomDTO>> GetItems(List<string> ids, CancellationToken cancellationToken = default);
    }
}