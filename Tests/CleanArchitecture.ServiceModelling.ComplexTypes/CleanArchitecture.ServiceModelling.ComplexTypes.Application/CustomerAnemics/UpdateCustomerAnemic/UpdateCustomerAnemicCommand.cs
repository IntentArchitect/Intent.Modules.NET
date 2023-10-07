using System;
using CleanArchitecture.ServiceModelling.ComplexTypes.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.ServiceModelling.ComplexTypes.Application.CustomerAnemics.UpdateCustomerAnemic
{
    public class UpdateCustomerAnemicCommand : IRequest, ICommand
    {
        public UpdateCustomerAnemicCommand(Guid id, string name, UpdateCustomerAnemicAddressDto address)
        {
            Id = id;
            Name = name;
            Address = address;
        }

        public Guid Id { get; private set; }
        public string Name { get; set; }
        public UpdateCustomerAnemicAddressDto Address { get; set; }

        public void SetId(Guid id)
        {
            Id = id;
        }
    }
}