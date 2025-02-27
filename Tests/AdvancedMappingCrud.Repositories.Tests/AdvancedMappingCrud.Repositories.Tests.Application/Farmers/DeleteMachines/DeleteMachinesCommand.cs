using System;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Farmers.DeleteMachines
{
    public class DeleteMachinesCommand : IRequest, ICommand
    {
        public DeleteMachinesCommand(Guid farmerId, Guid id)
        {
            FarmerId = farmerId;
            Id = id;
        }

        public Guid FarmerId { get; set; }
        public Guid Id { get; set; }
    }
}