using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.CompanyContacts.UpdateCompanyContact
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateCompanyContactCommandHandler : IRequestHandler<UpdateCompanyContactCommand>
    {
        private readonly ICompanyContactRepository _companyContactRepository;

        [IntentManaged(Mode.Merge)]
        public UpdateCompanyContactCommandHandler(ICompanyContactRepository companyContactRepository)
        {
            _companyContactRepository = companyContactRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateCompanyContactCommand request, CancellationToken cancellationToken)
        {
            var companyContact = await _companyContactRepository.FindByIdAsync(request.Id, cancellationToken);
            if (companyContact is null)
            {
                throw new NotFoundException($"Could not find CompanyContact '{request.Id}'");
            }

            companyContact.Contact = CreateOrUpdateContact(companyContact.Contact, request);
        }

        [IntentManaged(Mode.Fully)]
        private static Contact CreateOrUpdateContact(Contact? entity, UpdateCompanyContactCommand dto)
        {
            entity ??= new Contact();
            entity.FirstName = dto.FirstName;
            return entity;
        }
    }
}