using System;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using ProxyServiceTests.OriginalServices.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace ProxyServiceTests.OriginalServices.Application.Accounts.DeleteAccount
{
    public class DeleteAccountCommand : IRequest, ICommand
    {
        public DeleteAccountCommand(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}