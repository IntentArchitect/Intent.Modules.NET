using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using TrainingModel.Tests.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace TrainingModel.Tests.Application.Products.CaptureCategoriesProduct
{
    public class CaptureCategoriesProductCommand : IRequest, ICommand
    {
        public CaptureCategoriesProductCommand(Guid id, List<string> categoryNames)
        {
            Id = id;
            CategoryNames = categoryNames;
        }

        public Guid Id { get; set; }
        public List<string> CategoryNames { get; set; }
    }
}