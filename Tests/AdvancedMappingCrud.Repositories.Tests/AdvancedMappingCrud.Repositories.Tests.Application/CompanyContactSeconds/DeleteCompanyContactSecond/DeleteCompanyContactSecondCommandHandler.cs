using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.CompanyContactSeconds.DeleteCompanyContactSecond
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteCompanyContactSecondCommandHandler : IRequestHandler<DeleteCompanyContactSecondCommand>
    {
        private readonly ICompanyContactSecondRepository _companyContactSecondRepository;

        [IntentManaged(Mode.Merge)]
        public DeleteCompanyContactSecondCommandHandler(ICompanyContactSecondRepository companyContactSecondRepository)
        {
            _companyContactSecondRepository = companyContactSecondRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DeleteCompanyContactSecondCommand request, CancellationToken cancellationToken)
        {
            var companyContactSecond = await _companyContactSecondRepository.FindByIdAsync(request.Id, cancellationToken);
            if (companyContactSecond is null)
            {
                throw new NotFoundException($"Could not find CompanyContactSecond '{request.Id}'");
            }

            _companyContactSecondRepository.Remove(companyContactSecond);
        }
    }
}