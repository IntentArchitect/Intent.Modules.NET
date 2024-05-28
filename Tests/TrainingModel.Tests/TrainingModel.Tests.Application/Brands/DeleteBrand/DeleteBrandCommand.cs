using System;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using TrainingModel.Tests.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace TrainingModel.Tests.Application.Brands.DeleteBrand
{
    public class DeleteBrandCommand : IRequest, ICommand
    {
        public DeleteBrandCommand(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}