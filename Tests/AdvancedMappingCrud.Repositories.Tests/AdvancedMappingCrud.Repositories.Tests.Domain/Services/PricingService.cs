using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.DomainServices.DomainServiceImplementation", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Domain.Services
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class PricingService : IPricingService
    {
        [IntentManaged(Mode.Merge, Body = Mode.Ignore)]
        public PricingService()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<decimal> GetProductPriceAsync(Guid productId, CancellationToken cancellationToken = default)
        {
            // TODO: Implement GetProductPriceAsync (PricingService) functionality
            throw new NotImplementedException("Implement your domain service logic here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public decimal SumPrices(decimal prices)
        {
            // TODO: Implement SumPrices (PricingService) functionality
            throw new NotImplementedException("Implement your domain service logic here...");
        }
    }
}