using System;
using CleanArchitecture.ServiceModelling.ComplexTypes.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.ServiceModelling.ComplexTypes.Application.CustomerRiches.CreateCustomerRich
{
    public class CreateCustomerRichCommand : IRequest<Guid>, ICommand
    {
        public CreateCustomerRichCommand(CreateCustomerRichAddressDto address)
        {
            Address = address;
        }
        public CreateCustomerRichAddressDto Address { get; set; }
    }
}