using System;
using System.Collections.Generic;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Interfaces;
using AdvancedMappingCrud.Repositories.Tests.Domain;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.EntityListEnums.CreateEntityListEnum
{
    public class CreateEntityListEnumCommand : IRequest<Guid>, ICommand
    {
        public CreateEntityListEnumCommand(string name, IEnumerable<OrderStatus> orderStatuses)
        {
            Name = name;
            OrderStatuses = orderStatuses;
        }

        public string Name { get; set; }
        public IEnumerable<OrderStatus> OrderStatuses { get; set; }
    }
}