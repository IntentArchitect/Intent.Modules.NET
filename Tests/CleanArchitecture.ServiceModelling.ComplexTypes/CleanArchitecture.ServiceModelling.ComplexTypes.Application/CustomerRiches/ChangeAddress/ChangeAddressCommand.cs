using System;
using CleanArchitecture.ServiceModelling.ComplexTypes.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.ServiceModelling.ComplexTypes.Application.CustomerRiches.ChangeAddress
{
    public class ChangeAddressCommand : IRequest, ICommand
    {
        public ChangeAddressCommand(ChangeAddressDCDto address, Guid id)
        {
            Address = address;
            Id = id;
        }

        public ChangeAddressDCDto Address { get; set; }
        public Guid Id { get; set; }
    }
}