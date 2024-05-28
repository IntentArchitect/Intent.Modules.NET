using System;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using TrainingModel.Tests.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace TrainingModel.Tests.Application.Products.CreateProduct
{
    public class CreateProductCommand : IRequest<Guid>, ICommand
    {
        public CreateProductCommand(string name, Guid brandId, string description)
        {
            Name = name;
            BrandId = brandId;
            Description = description;
        }

        public string Name { get; set; }
        public Guid BrandId { get; set; }
        public string Description { get; set; }
    }
}