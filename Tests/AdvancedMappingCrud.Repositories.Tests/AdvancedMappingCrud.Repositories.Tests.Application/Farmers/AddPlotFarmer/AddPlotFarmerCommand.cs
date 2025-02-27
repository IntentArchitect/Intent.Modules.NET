using System;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Farmers.AddPlotFarmer
{
    public class AddPlotFarmerCommand : IRequest, ICommand
    {
        public AddPlotFarmerCommand(Guid id, AddPlotDto address)
        {
            Id = id;
            Address = address;
        }

        public Guid Id { get; set; }
        public AddPlotDto Address { get; set; }
    }
}