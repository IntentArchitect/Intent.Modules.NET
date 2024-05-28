using System;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using TrainingModel.Tests.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace TrainingModel.Tests.Application.Brands.UpdateBrand
{
    public class UpdateBrandCommand : IRequest, ICommand
    {
        public UpdateBrandCommand(string name, bool isActive, Guid id)
        {
            Name = name;
            IsActive = isActive;
            Id = id;
        }

        public string Name { get; set; }
        public bool IsActive { get; set; }
        public Guid Id { get; set; }
    }
}