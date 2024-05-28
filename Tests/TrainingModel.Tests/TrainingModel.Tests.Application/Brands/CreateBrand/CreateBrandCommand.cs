using System;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using TrainingModel.Tests.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace TrainingModel.Tests.Application.Brands.CreateBrand
{
    public class CreateBrandCommand : IRequest<Guid>, ICommand
    {
        public CreateBrandCommand(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}