using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using SharedKernel.Consumer.Tests.Domain.Common.Exceptions;
using SharedKernel.Kernel.Tests.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace SharedKernel.Consumer.Tests.Application.Currencies.UpdateCurrency
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateCurrencyCommandHandler : IRequestHandler<UpdateCurrencyCommand>
    {
        private readonly ICurrencyRepository _currencyRepository;

        [IntentManaged(Mode.Merge)]
        public UpdateCurrencyCommandHandler(ICurrencyRepository currencyRepository)
        {
            _currencyRepository = currencyRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateCurrencyCommand request, CancellationToken cancellationToken)
        {
            var currency = await _currencyRepository.FindByIdAsync(request.Id, cancellationToken);
            if (currency is null)
            {
                throw new NotFoundException($"Could not find Currency '{request.Id}'");
            }

            currency.CountryId = request.CountryId;
            currency.Name = request.Name;
            currency.Symbol = request.Symbol;
            currency.Description = request.Description;
        }
    }
}