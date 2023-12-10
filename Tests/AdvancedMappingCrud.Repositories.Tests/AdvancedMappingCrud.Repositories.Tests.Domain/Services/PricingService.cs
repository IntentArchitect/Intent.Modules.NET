using System;
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

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public decimal GetProductPrice(Guid productId)
        {
            throw new NotImplementedException("Implement your domain service logic here...");
        }
    }
}