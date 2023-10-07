using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Publish.CleanArchDapr.TestApplication.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace Publish.CleanArchDapr.TestApplication.Application.Customers.UpdateCustomer
{
    public class UpdateCustomerCommand : IRequest, ICommand
    {
        public UpdateCustomerCommand(Guid id)
        {
            Id = id;
        }
        public Guid Id { get; private set; }

        public void SetId(Guid id)
        {
            if (Id == default)
            {
                Id = id;
            }
        }

    }
}