using System;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using TrainingModel.Tests.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace TrainingModel.Tests.Application.Products.UpdateProduct
{
    public class UpdateProductCommand : IRequest, ICommand
    {
        public UpdateProductCommand(string name, Guid brandId, string description, bool isActive, Guid id)
        {
            Name = name;
            BrandId = brandId;
            Description = description;
            IsActive = isActive;
            Id = id;
        }

        public string Name { get; set; }
        public Guid BrandId { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public Guid Id { get; set; }
    }
}