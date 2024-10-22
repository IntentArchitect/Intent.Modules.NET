using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using SharedKernel.Consumer.Tests.Domain.Common.Exceptions;
using SharedKernel.Kernel.Tests.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace SharedKernel.Consumer.Tests.Application.Currencies.DeleteCurrency
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteCurrencyCommandHandler : IRequestHandler<DeleteCurrencyCommand>
    {
        private readonly ICurrencyRepository _currencyRepository;

        [IntentManaged(Mode.Merge)]
        public DeleteCurrencyCommandHandler(ICurrencyRepository currencyRepository)
        {
            _currencyRepository = currencyRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DeleteCurrencyCommand request, CancellationToken cancellationToken)
        {
            var currency = await _currencyRepository.FindByIdAsync(request.Id, cancellationToken);
            if (currency is null)
            {
                throw new NotFoundException($"Could not find Currency '{request.Id}'");
            }

            _currencyRepository.Remove(currency);
        }
    }
}