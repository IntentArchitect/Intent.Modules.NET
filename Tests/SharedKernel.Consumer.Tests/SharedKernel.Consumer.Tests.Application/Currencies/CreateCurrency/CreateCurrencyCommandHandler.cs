using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using SharedKernel.Kernel.Tests.Domain.Entities;
using SharedKernel.Kernel.Tests.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace SharedKernel.Consumer.Tests.Application.Currencies.CreateCurrency
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateCurrencyCommandHandler : IRequestHandler<CreateCurrencyCommand, Guid>
    {
        private readonly ICurrencyRepository _currencyRepository;

        [IntentManaged(Mode.Merge)]
        public CreateCurrencyCommandHandler(ICurrencyRepository currencyRepository)
        {
            _currencyRepository = currencyRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateCurrencyCommand request, CancellationToken cancellationToken)
        {
            var currency = new Currency(
                countryId: request.CountryId,
                name: request.Name,
                symbol: request.Symbol);

            _currencyRepository.Add(currency);
            await _currencyRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return currency.Id;
        }
    }
}