using System;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.CompanyContacts.CreateCompanyContact
{
    public class CreateCompanyContactCommand : IRequest<Guid>, ICommand
    {
        public CreateCompanyContactCommand(Guid contactId)
        {
            ContactId = contactId;
        }

        public Guid ContactId { get; set; }
    }
}