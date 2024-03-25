using System;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Repositories.MappableStoredProcs;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Application.MappableStoredProcs.GetMockEntity
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetMockEntityHandler : IRequestHandler<GetMockEntity, MockEntityDto>
    {
        private readonly IMockEntityRepository _mockEntityRepository;

        [IntentManaged(Mode.Merge)]
        public GetMockEntityHandler(IMockEntityRepository mockEntityRepository)
        {
            _mockEntityRepository = mockEntityRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<MockEntityDto> Handle(GetMockEntity request, CancellationToken cancellationToken)
        {
            var result = _mockEntityRepository.GetMockEntityById(request.Id);

            throw new NotImplementedException("Implement return type mapping...");
        }
    }
}