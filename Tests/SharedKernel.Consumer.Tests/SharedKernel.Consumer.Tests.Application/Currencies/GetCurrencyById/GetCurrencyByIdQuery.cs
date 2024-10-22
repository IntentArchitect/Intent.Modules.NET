using System;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using SharedKernel.Consumer.Tests.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace SharedKernel.Consumer.Tests.Application.Currencies.GetCurrencyById
{
    public class GetCurrencyByIdQuery : IRequest<CurrencyDto>, IQuery
    {
        public GetCurrencyByIdQuery(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}