using System;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using ProxyServiceTests.OriginalServices.Application.Common.Interfaces;
using ProxyServiceTests.OriginalServices.Application.Common.Security;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace ProxyServiceTests.OriginalServices.Application.Accounts.DeleteAccount
{
    [Authorize]
    public class DeleteAccountCommand : IRequest, ICommand
    {
        public DeleteAccountCommand(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}