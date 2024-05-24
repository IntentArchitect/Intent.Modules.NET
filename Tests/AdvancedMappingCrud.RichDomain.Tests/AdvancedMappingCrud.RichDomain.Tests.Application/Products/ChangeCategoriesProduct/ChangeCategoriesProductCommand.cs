using System;
using System.Collections.Generic;
using AdvancedMappingCrud.RichDomain.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Application.Products.ChangeCategoriesProduct
{
    public class ChangeCategoriesProductCommand : IRequest, ICommand
    {
        public ChangeCategoriesProductCommand(Guid id, List<string> categoryNames)
        {
            Id = id;
            CategoryNames = categoryNames;
        }

        public Guid Id { get; set; }
        public List<string> CategoryNames { get; set; }
    }
}