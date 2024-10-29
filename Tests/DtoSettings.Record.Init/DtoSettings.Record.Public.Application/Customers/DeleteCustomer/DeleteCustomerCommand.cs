using System;
using DtoSettings.Record.Public.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace DtoSettings.Record.Public.Application.Customers.DeleteCustomer
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