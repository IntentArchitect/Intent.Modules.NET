using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.CompanyContacts.DeleteCompanyContact
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteCompanyContactCommandHandler : IRequestHandler<DeleteCompanyContactCommand>
    {
        private readonly ICompanyContactRepository _companyContactRepository;

        [IntentManaged(Mode.Merge)]
        public DeleteCompanyContactCommandHandler(ICompanyContactRepository companyContactRepository)
        {
            _companyContactRepository = companyContactRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DeleteCompanyContactCommand request, CancellationToken cancellationToken)
        {
            var companyContact = await _companyContactRepository.FindByIdAsync(request.Id, cancellationToken);
            if (companyContact is null)
            {
                throw new NotFoundException($"Could not find CompanyContact '{request.Id}'");
            }

            _companyContactRepository.Remove(companyContact);
        }
    }
}