using System;
using CleanArchitecture.ServiceModelling.ComplexTypes.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.ServiceModelling.ComplexTypes.Application.CustomerAnemics.CreateCustomerAnemic
{
    public class CreateCustomerAnemicCommand : IRequest<Guid>, ICommand
    {
        public CreateCustomerAnemicCommand(string name, CreateCustomerAnemicAddressDto address)
        {
            Name = name;
            Address = address;
        }

        public string Name { get; set; }
        public CreateCustomerAnemicAddressDto Address { get; set; }
    }
}