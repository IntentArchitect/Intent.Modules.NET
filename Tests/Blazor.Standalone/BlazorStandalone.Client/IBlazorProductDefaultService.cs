using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BlazorStandalone.Client.Contracts.BlazorProductService.Services;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.ServiceContract", Version = "2.0")]

namespace BlazorStandalone.Client
{
    public interface IBlazorProductDefaultService : IDisposable
    {
        Task<List<ProductDto>> GetProductsAsync(CancellationToken cancellationToken = default);
    }
}