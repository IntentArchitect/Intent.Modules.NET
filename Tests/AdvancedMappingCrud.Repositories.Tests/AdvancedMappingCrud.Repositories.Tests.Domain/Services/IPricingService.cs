using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.DomainServices.DomainServiceInterface", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Domain.Services
{
    public interface IPricingService
    {
        Task<decimal> GetProductPriceAsync(Guid productId, CancellationToken cancellationToken = default);
        decimal SumPrices(decimal prices);
    }
}