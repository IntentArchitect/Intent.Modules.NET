using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using SharedKernel.Consumer.Tests.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace SharedKernel.Consumer.Tests.Application.Currencies.GetCurrencies
{
    public class GetCurrenciesQuery : IRequest<List<CurrencyDto>>, IQuery
    {
        public GetCurrenciesQuery()
        {
        }
    }
}