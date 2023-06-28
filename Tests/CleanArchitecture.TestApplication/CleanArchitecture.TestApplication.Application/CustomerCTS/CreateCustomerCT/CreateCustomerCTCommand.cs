using System;
using CleanArchitecture.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.CustomerCTS.CreateCustomerCT
{
    public class CreateCustomerCTCommand : IRequest<Guid>, ICommand
    {
        public CreateCustomerCTCommand(string name, CreateCustomerCTAddressCTDto addressCT)
        {
            Name = name;
            AddressCT = addressCT;
        }

        public string Name { get; set; }
        public CreateCustomerCTAddressCTDto AddressCT { get; set; }
    }
}