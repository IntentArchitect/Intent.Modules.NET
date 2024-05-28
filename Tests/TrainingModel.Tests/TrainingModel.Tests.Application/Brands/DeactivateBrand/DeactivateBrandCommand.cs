using System;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using TrainingModel.Tests.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace TrainingModel.Tests.Application.Brands.DeactivateBrand
{
    public class DeactivateBrandCommand : IRequest, ICommand
    {
        public DeactivateBrandCommand(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}