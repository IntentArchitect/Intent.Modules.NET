using System;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Customers.PatchCustomer
{
    public class PatchCustomerCommand : IRequest, ICommand
    {
        public PatchCustomerCommand(Guid id, string? name, string? surname, bool? isActive, bool? newsletter, bool? specials)
        {
            Id = id;
            Name = name;
            Surname = surname;
            IsActive = isActive;
            Newsletter = newsletter;
            Specials = specials;
        }

        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public bool? IsActive { get; set; }
        public bool? Newsletter { get; set; }
        public bool? Specials { get; set; }
    }
}