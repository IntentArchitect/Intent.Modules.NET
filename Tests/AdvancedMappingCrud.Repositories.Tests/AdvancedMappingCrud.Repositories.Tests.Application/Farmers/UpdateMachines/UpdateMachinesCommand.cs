using System;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Farmers.UpdateMachines
{
    public class UpdateMachinesCommand : IRequest, ICommand
    {
        public UpdateMachinesCommand(Guid farmerId, string name, Guid id)
        {
            FarmerId = farmerId;
            Name = name;
            Id = id;
        }

        public Guid FarmerId { get; set; }
        public string Name { get; set; }
        public Guid Id { get; set; }
    }
}