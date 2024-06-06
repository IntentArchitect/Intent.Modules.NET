using System;
using CleanArchitecture.Comprehensive.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.DDD.CreateTransaction
{
    public class CreateTransactionCommand : IRequest, ICommand
    {
        public CreateTransactionCommand(CreateMoneyDto current, string description, Guid accountId, CreateAccountDto account)
        {
            Current = current;
            Description = description;
            AccountId = accountId;
            Account = account;
        }

        public CreateMoneyDto Current { get; set; }
        public string Description { get; set; }
        public Guid AccountId { get; set; }
        public CreateAccountDto Account { get; set; }
    }
}