using System;
using System.Collections.Generic;
using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.Products.CreateProduct
{
    public class CreateProductCommand : IRequest<Guid>, ICommand
    {
        public CreateProductCommand(string name, List<string> categoryNames)
        {
            Name = name;
            CategoryNames = categoryNames;
        }

        public string Name { get; set; }
        public List<string> CategoryNames { get; set; }
    }
}