using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.CompanyContacts.CreateCompanyContact
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateCompanyContactCommandHandler : IRequestHandler<CreateCompanyContactCommand, Guid>
    {
        private readonly ICompanyContactRepository _companyContactRepository;

        [IntentManaged(Mode.Merge)]
        public CreateCompanyContactCommandHandler(ICompanyContactRepository companyContactRepository)
        {
            _companyContactRepository = companyContactRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateCompanyContactCommand request, CancellationToken cancellationToken)
        {
            var companyContact = new CompanyContact
            {
                ContactId = request.ContactId
            };

            _companyContactRepository.Add(companyContact);
            await _companyContactRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return companyContact.Id;
        }
    }
}