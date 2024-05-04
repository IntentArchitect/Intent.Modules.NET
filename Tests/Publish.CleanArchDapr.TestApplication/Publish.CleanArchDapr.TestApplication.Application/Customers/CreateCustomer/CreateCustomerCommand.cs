using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Publish.CleanArchDapr.TestApplication.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace Publish.CleanArchDapr.TestApplication.Application.Customers.CreateCustomer
{
    public class CreateCustomerCommand : IRequest<Guid>, ICommand
    {
        public CreateCustomerCommand(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}