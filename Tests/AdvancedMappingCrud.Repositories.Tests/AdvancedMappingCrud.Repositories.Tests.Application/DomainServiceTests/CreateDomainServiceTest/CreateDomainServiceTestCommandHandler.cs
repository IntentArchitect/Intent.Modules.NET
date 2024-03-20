using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities.DomainServices;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories.DomainServices;
using AdvancedMappingCrud.Repositories.Tests.Domain.Services.DomainServices;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.DomainServiceTests.CreateDomainServiceTest
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateDomainServiceTestCommandHandler : IRequestHandler<CreateDomainServiceTestCommand, Guid>
    {
        private readonly IDomainServiceTestRepository _domainServiceTestRepository;
        private readonly IMyDomainService _domainService;

        [IntentManaged(Mode.Merge)]
        public CreateDomainServiceTestCommandHandler(IDomainServiceTestRepository domainServiceTestRepository,
            IMyDomainService domainService)
        {
            _domainServiceTestRepository = domainServiceTestRepository;
            _domainService = domainService;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateDomainServiceTestCommand request, CancellationToken cancellationToken)
        {
            var newDomainServiceTest = new DomainServiceTest(_domainService);

            _domainServiceTestRepository.Add(newDomainServiceTest);
            await _domainServiceTestRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return newDomainServiceTest.Id;
        }
    }
}