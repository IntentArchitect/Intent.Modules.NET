using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Integration.HttpClients.TestApplication.Application.Invoices;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace Integration.HttpClients.TestApplication.Application.Interfaces
{

    public interface IInvoiceService : IDisposable
    {

        Task Create(InvoiceCreateDTO dto);

        Task<InvoiceDTO> FindById(Guid id);

        Task<List<InvoiceDTO>> FindAll();

        Task Update(Guid id, InvoiceUpdateDTO dto);

        Task Delete(Guid id);

        Task<InvoiceDTO> QueryParamOp(string param1, int param2);

        Task HeaderParamOp(string param1);

        Task FormParamOp(string param1, int param2);

        Task RouteParamOp(string param1);

        Task BodyParamOp(InvoiceDTO param1);

        Task ThrowsException();

        Task<Guid> GetWrappedPrimitiveGuid();

        Task<string> GetWrappedPrimitiveString();

        Task<int> GetWrappedPrimitiveInt();

        Task<Guid> GetPrimitiveGuid();

        Task<string> GetPrimitiveString();

        Task<int> GetPrimitiveInt();

        Task<List<string>> GetPrimitiveStringList();

        Task NonHttpSettingsOperation();

        Task<InvoiceDTO> GetInvoiceOpWithReturnTypeWrapped();

    }
}