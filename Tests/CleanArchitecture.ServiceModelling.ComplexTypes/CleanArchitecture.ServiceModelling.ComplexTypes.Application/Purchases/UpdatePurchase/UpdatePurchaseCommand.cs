using System;
using CleanArchitecture.ServiceModelling.ComplexTypes.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.ServiceModelling.ComplexTypes.Application.Purchases.UpdatePurchase
{
    public class UpdatePurchaseCommand : IRequest, ICommand
    {
        public UpdatePurchaseCommand(Guid id, UpdatePurchaseMoneyDto cost)
        {
            Id = id;
            Cost = cost;
        }

        public Guid Id { get; set; }
        public UpdatePurchaseMoneyDto Cost { get; set; }
    }
}