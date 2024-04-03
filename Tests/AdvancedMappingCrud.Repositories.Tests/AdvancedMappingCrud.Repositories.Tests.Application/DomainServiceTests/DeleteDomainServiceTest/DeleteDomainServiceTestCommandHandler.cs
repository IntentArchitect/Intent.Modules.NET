using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories.DomainServices;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.DomainServiceTests.DeleteDomainServiceTest
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteDomainServiceTestCommandHandler : IRequestHandler<DeleteDomainServiceTestCommand>
    {
        private readonly IDomainServiceTestRepository _domainServiceTestRepository;

        [IntentManaged(Mode.Merge)]
        public DeleteDomainServiceTestCommandHandler(IDomainServiceTestRepository domainServiceTestRepository)
        {
            _domainServiceTestRepository = domainServiceTestRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DeleteDomainServiceTestCommand request, CancellationToken cancellationToken)
        {
            var existingDomainServiceTest = await _domainServiceTestRepository.FindByIdAsync(request.Id, cancellationToken);
            if (existingDomainServiceTest is null)
            {
                throw new NotFoundException($"Could not find DomainServiceTest '{request.Id}'");
            }

            _domainServiceTestRepository.Remove(existingDomainServiceTest);
        }
    }
}