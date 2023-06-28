using System;
using CleanArchitecture.ServiceModelling.ComplexTypes.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.ServiceModelling.ComplexTypes.Application.Purchases.DeletePurchase
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