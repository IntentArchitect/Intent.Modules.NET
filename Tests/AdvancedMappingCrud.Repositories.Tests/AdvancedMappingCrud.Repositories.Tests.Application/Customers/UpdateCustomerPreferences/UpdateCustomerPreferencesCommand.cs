using System;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Customers.UpdateCustomerPreferences
{
    public class UpdateCustomerPreferencesCommand : IRequest, ICommand
    {
        public UpdateCustomerPreferencesCommand(Guid customerId, bool newsletter, bool specials)
        {
            CustomerId = customerId;
            Newsletter = newsletter;
            Specials = specials;
        }

        public Guid CustomerId { get; set; }
        public bool Newsletter { get; set; }
        public bool Specials { get; set; }
    }
}