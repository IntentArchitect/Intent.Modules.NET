using System;
using Google.Cloud.Storage.Multitenancy.SeperateAccount.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace Google.Cloud.Storage.Multitenancy.SeperateAccount.Tests.Application.Customers.CreateCustomer
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