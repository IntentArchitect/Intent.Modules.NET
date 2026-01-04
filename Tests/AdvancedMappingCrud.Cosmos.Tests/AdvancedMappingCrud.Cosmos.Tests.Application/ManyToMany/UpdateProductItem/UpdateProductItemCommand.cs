using System;
using System.Collections.Generic;
using AdvancedMappingCrud.Cosmos.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.ManyToMany.UpdateProductItem
{
    public class UpdateProductItemCommand : IRequest, ICommand
    {
        public UpdateProductItemCommand(Guid id, string name, List<Guid> tagIds, List<Guid> categoryIds)
        {
            Id = id;
            Name = name;
            TagIds = tagIds;
            CategoryIds = categoryIds;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<Guid> TagIds { get; set; }
        public List<Guid> CategoryIds { get; set; }
    }
}