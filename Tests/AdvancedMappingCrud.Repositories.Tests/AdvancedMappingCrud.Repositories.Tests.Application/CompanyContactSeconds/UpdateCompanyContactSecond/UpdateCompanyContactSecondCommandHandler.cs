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

namespace AdvancedMappingCrud.Repositories.Tests.Application.CompanyContactSeconds.UpdateCompanyContactSecond
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateCompanyContactSecondCommandHandler : IRequestHandler<UpdateCompanyContactSecondCommand>
    {
        private readonly ICompanyContactSecondRepository _companyContactSecondRepository;

        [IntentManaged(Mode.Merge)]
        public UpdateCompanyContactSecondCommandHandler(ICompanyContactSecondRepository companyContactSecondRepository)
        {
            _companyContactSecondRepository = companyContactSecondRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateCompanyContactSecondCommand request, CancellationToken cancellationToken)
        {
            var companyContactSecond = await _companyContactSecondRepository.FindByIdAsync(request.Id, cancellationToken);
            if (companyContactSecond is null)
            {
                throw new NotFoundException($"Could not find CompanyContactSecond '{request.Id}'");
            }

            companyContactSecond.ContactSecondId = request.ContactSecondId;
            companyContactSecond.ContactSecond = CreateOrUpdateContactSecond(companyContactSecond.ContactSecond, request);
        }

        [IntentManaged(Mode.Fully)]
        private static ContactSecond CreateOrUpdateContactSecond(
            ContactSecond? entity,
            UpdateCompanyContactSecondCommand dto)
        {
            entity ??= new ContactSecond();
            entity.ContactName = dto.ContactName;
            entity.ContactDetailsSecond.Name = dto.Name;
            return entity;
        }
    }
}