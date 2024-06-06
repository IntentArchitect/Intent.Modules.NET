using System;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Solace.Tests.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace Solace.Tests.Application.Accounts.UpdateAccount
{
    public class UpdateAccountCommand : IRequest, ICommand
    {
        public UpdateAccountCommand(Guid customerId, Guid id)
        {
            CustomerId = customerId;
            Id = id;
        }

        public Guid CustomerId { get; set; }
        public Guid Id { get; set; }
    }
}