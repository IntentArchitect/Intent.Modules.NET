using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using SharedKernel.Kernel.Tests.Domain.Services;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace SharedKernel.Consumer.Tests.Application.Currencies.GetDefaultCurrency
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetDefaultCurrencyQueryHandler : IRequestHandler<GetDefaultCurrencyQuery, CurrencyDto>
    {
        private readonly ICurrencyService _currencyService;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetDefaultCurrencyQueryHandler(ICurrencyService currencyService, IMapper mapper)
        {
            _currencyService = currencyService;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<CurrencyDto> Handle(GetDefaultCurrencyQuery request, CancellationToken cancellationToken)
        {
            var result = await _currencyService.GetDefaultCurrencyAsync(request.CountryId);
            return result.MapToCurrencyDto(_mapper);
        }
    }
}