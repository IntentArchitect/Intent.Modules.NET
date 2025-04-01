using System;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.CompanyContacts.UpdateCompanyContact
{
    public class UpdateCompanyContactCommand : IRequest, ICommand
    {
        public UpdateCompanyContactCommand(Guid contactId, Guid id, string firstName)
        {
            ContactId = contactId;
            Id = id;
            FirstName = firstName;
        }

        public Guid ContactId { get; set; }
        public Guid Id { get; set; }
        public string FirstName { get; set; }
    }
}