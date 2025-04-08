using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.CompanyContactSeconds.CreateCompanyContactSecond
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateCompanyContactSecondCommandHandler : IRequestHandler<CreateCompanyContactSecondCommand, Guid>
    {
        private readonly ICompanyContactSecondRepository _companyContactSecondRepository;

        [IntentManaged(Mode.Merge)]
        public CreateCompanyContactSecondCommandHandler(ICompanyContactSecondRepository companyContactSecondRepository)
        {
            _companyContactSecondRepository = companyContactSecondRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateCompanyContactSecondCommand request, CancellationToken cancellationToken)
        {
            var companyContactSecond = new CompanyContactSecond
            {
                ContactSecondId = request.ContactSecondId
            };

            _companyContactSecondRepository.Add(companyContactSecond);
            await _companyContactSecondRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return companyContactSecond.Id;
        }
    }
}