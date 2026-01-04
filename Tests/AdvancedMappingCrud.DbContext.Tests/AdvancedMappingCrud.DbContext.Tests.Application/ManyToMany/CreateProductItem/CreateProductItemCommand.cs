using System;
using System.Collections.Generic;
using AdvancedMappingCrud.DbContext.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.DbContext.Tests.Application.ManyToMany.CreateProductItem
{
    public class CreateProductItemCommand : IRequest, ICommand
    {
        public CreateProductItemCommand(string name, List<Guid> tagIds, List<Guid> categoryIds)
        {
            Name = name;
            TagIds = tagIds;
            CategoryIds = categoryIds;
        }

        public string Name { get; set; }
        public List<Guid> TagIds { get; set; }
        public List<Guid> CategoryIds { get; set; }
    }
}