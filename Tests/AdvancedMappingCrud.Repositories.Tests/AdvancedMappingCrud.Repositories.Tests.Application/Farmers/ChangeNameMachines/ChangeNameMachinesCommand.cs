using System;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Farmers.ChangeNameMachines
{
    public class ChangeNameMachinesCommand : IRequest, ICommand
    {
        public ChangeNameMachinesCommand(Guid farmerId, Guid id, string name)
        {
            FarmerId = farmerId;
            Id = id;
            Name = name;
        }

        public Guid FarmerId { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}