using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using SharedKernel.Kernel.Tests.Domain.Common.Exceptions;
using SharedKernel.Kernel.Tests.Domain.Entities;
using SharedKernel.Kernel.Tests.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.DomainServices.DomainServiceImplementation", Version = "1.0")]

namespace SharedKernel.Kernel.Tests.Domain.Services
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CurrencyService : ICurrencyService
    {
        private readonly ICurrencyRepository _currencyRepository;

        [IntentManaged(Mode.Merge, Body = Mode.Ignore)]
        public CurrencyService(ICurrencyRepository currencyRepository)
        {
            _currencyRepository = currencyRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public Currency GetDefaultCurrency(Guid countryId)
        {
            var currencies = _currencyRepository.FindAllAsync().GetAwaiter().GetResult();
            if (!@currencies.Any())
            {
                throw new NotFoundException($"No Currencies");
            }
            return currencies.First();
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<Currency> GetDefaultCurrencyAsync(Guid countryId, CancellationToken cancellationToken = default)
        {
            var currencies = await _currencyRepository.FindAllAsync(cancellationToken);
            if (@currencies.Any())
            {
                throw new NotFoundException($"No Currencies");
            }
            return currencies.First();
        }
    }
}