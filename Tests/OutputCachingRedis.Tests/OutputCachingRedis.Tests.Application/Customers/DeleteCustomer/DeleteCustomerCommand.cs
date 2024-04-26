using System;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using OutputCachingRedis.Tests.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace OutputCachingRedis.Tests.Application.Customers.DeleteCustomer
{
    public class DeleteCustomerCommand : IRequest, ICommand
    {
        public DeleteCustomerCommand(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}