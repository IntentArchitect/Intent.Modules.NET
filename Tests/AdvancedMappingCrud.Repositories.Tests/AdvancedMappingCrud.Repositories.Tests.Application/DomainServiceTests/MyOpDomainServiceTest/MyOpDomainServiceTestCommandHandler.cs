using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories.DomainServices;
using AdvancedMappingCrud.Repositories.Tests.Domain.Services.DomainServices;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.DomainServiceTests.MyOpDomainServiceTest
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class MyOpDomainServiceTestCommandHandler : IRequestHandler<MyOpDomainServiceTestCommand>
    {
        private readonly IDomainServiceTestRepository _domainServiceTestRepository;
        private readonly IMyDomainService _domainService;

        [IntentManaged(Mode.Merge)]
        public MyOpDomainServiceTestCommandHandler(IDomainServiceTestRepository domainServiceTestRepository,
            IMyDomainService domainService)
        {
            _domainServiceTestRepository = domainServiceTestRepository;
            _domainService = domainService;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(MyOpDomainServiceTestCommand request, CancellationToken cancellationToken)
        {
            var existingDomainServiceTest = await _domainServiceTestRepository.FindByIdAsync(request.Id, cancellationToken);
            if (existingDomainServiceTest is null)
            {
                throw new NotFoundException($"Could not find DomainServiceTest '{request.Id}'");
            }

            existingDomainServiceTest.MyOp(_domainService);
        }
    }
}