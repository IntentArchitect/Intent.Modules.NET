using System;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using TrainingModel.Tests.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace TrainingModel.Tests.Application.Products.AddPriceProduct
{
    public class AddPriceProductCommand : IRequest, ICommand
    {
        public AddPriceProductCommand(Guid id, DateTime activeFrom, decimal price)
        {
            Id = id;
            ActiveFrom = activeFrom;
            Price = price;
        }

        public Guid Id { get; set; }
        public DateTime ActiveFrom { get; set; }
        public decimal Price { get; set; }
    }
}