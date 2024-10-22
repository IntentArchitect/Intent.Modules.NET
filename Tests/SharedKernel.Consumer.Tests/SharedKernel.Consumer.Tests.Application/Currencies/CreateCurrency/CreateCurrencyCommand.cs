using System;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using SharedKernel.Consumer.Tests.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace SharedKernel.Consumer.Tests.Application.Currencies.CreateCurrency
{
    public class CreateCurrencyCommand : IRequest<Guid>, ICommand
    {
        public CreateCurrencyCommand(Guid countryId, string name, string symbol)
        {
            CountryId = countryId;
            Name = name;
            Symbol = symbol;
        }

        public Guid CountryId { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }
    }
}