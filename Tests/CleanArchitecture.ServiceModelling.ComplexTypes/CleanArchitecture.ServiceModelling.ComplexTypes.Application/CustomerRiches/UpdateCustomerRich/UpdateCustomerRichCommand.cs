using System;
using CleanArchitecture.ServiceModelling.ComplexTypes.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.ServiceModelling.ComplexTypes.Application.CustomerRiches.UpdateCustomerRich
{
    public class UpdateCustomerRichCommand : IRequest, ICommand
    {
        public UpdateCustomerRichCommand(Guid id, UpdateCustomerRichAddressDto address)
        {
            Id = id;
            Address = address;
        }

        public Guid Id { get; set; }
        public UpdateCustomerRichAddressDto Address { get; set; }
    }
}