using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using IntegrationTesting.Tests.Domain.Common.Exceptions;
using IntegrationTesting.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace IntegrationTesting.Tests.Application.DiffIds.GetDiffIdById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetDiffIdByIdQueryHandler : IRequestHandler<GetDiffIdByIdQuery, DiffIdDto>
    {
        private readonly IDiffIdRepository _diffIdRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetDiffIdByIdQueryHandler(IDiffIdRepository diffIdRepository, IMapper mapper)
        {
            _diffIdRepository = diffIdRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<DiffIdDto> Handle(GetDiffIdByIdQuery request, CancellationToken cancellationToken)
        {
            var diffId = await _diffIdRepository.FindByIdAsync(request.Id, cancellationToken);
            if (diffId is null)
            {
                throw new NotFoundException($"Could not find DiffId '{request.Id}'");
            }
            return diffId.MapToDiffIdDto(_mapper);
        }
    }
}