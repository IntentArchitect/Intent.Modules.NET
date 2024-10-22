using System;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using SharedKernel.Consumer.Tests.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace SharedKernel.Consumer.Tests.Application.Currencies.GetDefaultCurrency
{
    public class GetDefaultCurrencyQuery : IRequest<CurrencyDto>, IQuery
    {
        public GetDefaultCurrencyQuery(Guid countryId)
        {
            CountryId = countryId;
        }

        public Guid CountryId { get; set; }
    }
}