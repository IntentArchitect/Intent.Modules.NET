using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using IntegrationTesting.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace IntegrationTesting.Tests.Application.BadSignatures.GetBadSignatures
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetBadSignaturesQueryHandler : IRequestHandler<GetBadSignaturesQuery, List<BadSignaturesDto>>
    {
        private readonly IBadSignaturesRepository _badSignaturesRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetBadSignaturesQueryHandler(IBadSignaturesRepository badSignaturesRepository, IMapper mapper)
        {
            _badSignaturesRepository = badSignaturesRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<BadSignaturesDto>> Handle(
            GetBadSignaturesQuery request,
            CancellationToken cancellationToken)
        {
            var badSignatures = await _badSignaturesRepository.FindAllAsync(cancellationToken);
            return badSignatures.MapToBadSignaturesDtoList(_mapper);
        }
    }
}