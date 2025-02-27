using System;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Farmers.CreateMachines
{
    public class CreateMachinesCommand : IRequest<Guid>, ICommand
    {
        public CreateMachinesCommand(Guid farmerId, string name)
        {
            FarmerId = farmerId;
            Name = name;
        }

        public Guid FarmerId { get; set; }
        public string Name { get; set; }
    }
}