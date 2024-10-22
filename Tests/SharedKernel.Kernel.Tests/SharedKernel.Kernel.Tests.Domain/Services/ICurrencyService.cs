using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using SharedKernel.Kernel.Tests.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.DomainServices.DomainServiceInterface", Version = "1.0")]

namespace SharedKernel.Kernel.Tests.Domain.Services
{
    public interface ICurrencyService
    {
        Currency GetDefaultCurrency(Guid countryId);
        Task<Currency> GetDefaultCurrencyAsync(Guid countryId, CancellationToken cancellationToken = default);
    }
}