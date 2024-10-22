using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using SharedKernel.Consumer.Tests.Domain.Common.Exceptions;
using SharedKernel.Kernel.Tests.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace SharedKernel.Consumer.Tests.Application.Currencies.GetCurrencyById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetCurrencyByIdQueryHandler : IRequestHandler<GetCurrencyByIdQuery, CurrencyDto>
    {
        private readonly ICurrencyRepository _currencyRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetCurrencyByIdQueryHandler(ICurrencyRepository currencyRepository, IMapper mapper)
        {
            _currencyRepository = currencyRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<CurrencyDto> Handle(GetCurrencyByIdQuery request, CancellationToken cancellationToken)
        {
            var currency = await _currencyRepository.FindByIdAsync(request.Id, cancellationToken);
            if (currency is null)
            {
                throw new NotFoundException($"Could not find Currency '{request.Id}'");
            }
            return currency.MapToCurrencyDto(_mapper);
        }
    }
}