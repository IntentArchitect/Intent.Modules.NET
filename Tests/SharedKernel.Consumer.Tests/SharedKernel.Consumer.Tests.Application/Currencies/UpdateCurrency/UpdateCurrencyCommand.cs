using System;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using SharedKernel.Consumer.Tests.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace SharedKernel.Consumer.Tests.Application.Currencies.UpdateCurrency
{
    public class UpdateCurrencyCommand : IRequest, ICommand
    {
        public UpdateCurrencyCommand(Guid countryId, string name, string symbol, string description, Guid id)
        {
            CountryId = countryId;
            Name = name;
            Symbol = symbol;
            Description = description;
            Id = id;
        }

        public Guid CountryId { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }
        public string Description { get; set; }
        public Guid Id { get; set; }
    }
}