using System;
using System.Collections.Generic;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Interfaces;
using AdvancedMappingCrud.Repositories.Tests.Domain;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.EntityListEnums.UpdateEntityListEnum
{
    public class UpdateEntityListEnumCommand : IRequest, ICommand
    {
        public UpdateEntityListEnumCommand(string name, IEnumerable<OrderStatus> orderStatuses, Guid id)
        {
            Name = name;
            OrderStatuses = orderStatuses;
            Id = id;
        }

        public string Name { get; set; }
        public IEnumerable<OrderStatus> OrderStatuses { get; set; }
        public Guid Id { get; set; }
    }
}