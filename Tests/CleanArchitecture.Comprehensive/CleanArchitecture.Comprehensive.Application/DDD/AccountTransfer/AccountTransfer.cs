using System;
using CleanArchitecture.Comprehensive.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.DDD.AccountTransfer
{
    public class AccountTransfer : IRequest, ICommand
    {
        public AccountTransfer(Guid id, string description, decimal amount, string currency)
        {
            Id = id;
            Description = description;
            Amount = amount;
            Currency = currency;
        }

        public Guid Id { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
    }
}