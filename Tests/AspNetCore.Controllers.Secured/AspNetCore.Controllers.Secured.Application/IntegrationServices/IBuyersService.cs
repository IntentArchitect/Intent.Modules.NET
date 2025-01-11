using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AspNetCore.Controllers.Secured.Application.IntegrationServices.Contracts.PackageLevelSecurity.Buyers;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.ServiceContract", Version = "2.0")]

namespace AspNetCore.Controllers.Secured.Application.IntegrationServices
{
    public interface IBuyersService : IDisposable
    {
        Task<Guid> CreateBuyerAsync(CreateBuyerCommand command, CancellationToken cancellationToken = default);
        Task DeleteBuyerAsync(Guid id, CancellationToken cancellationToken = default);
        Task UpdateBuyerAsync(Guid id, UpdateBuyerCommand command, CancellationToken cancellationToken = default);
        Task<BuyerDto> GetBuyerByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<BuyerDto>> GetBuyersAsync(CancellationToken cancellationToken = default);
    }
}