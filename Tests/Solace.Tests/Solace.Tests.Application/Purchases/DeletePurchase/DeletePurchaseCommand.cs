using System;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Solace.Tests.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace Solace.Tests.Application.Purchases.DeletePurchase
{
    public class DeletePurchaseCommand : IRequest, ICommand
    {
        public DeletePurchaseCommand(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}