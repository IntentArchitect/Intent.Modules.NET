using System;
using CleanArchitecture.ServiceModelling.ComplexTypes.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.ServiceModelling.ComplexTypes.Application.Purchases.CreatePurchase
{
    public class CreatePurchaseCommand : IRequest<PurchaseDto>, ICommand
    {
        public CreatePurchaseCommand(CreatePurchaseMoneyDto cost)
        {
            Cost = cost;
        }

        public CreatePurchaseMoneyDto Cost { get; set; }
    }
}