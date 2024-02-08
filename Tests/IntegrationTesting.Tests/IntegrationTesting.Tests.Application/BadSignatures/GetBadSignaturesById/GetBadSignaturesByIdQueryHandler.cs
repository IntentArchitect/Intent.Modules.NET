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

namespace IntegrationTesting.Tests.Application.BadSignatures.GetBadSignaturesById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetBadSignaturesByIdQueryHandler : IRequestHandler<GetBadSignaturesByIdQuery, BadSignaturesDto>
    {
        private readonly IBadSignaturesRepository _badSignaturesRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetBadSignaturesByIdQueryHandler(IBadSignaturesRepository badSignaturesRepository, IMapper mapper)
        {
            _badSignaturesRepository = badSignaturesRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<BadSignaturesDto> Handle(GetBadSignaturesByIdQuery request, CancellationToken cancellationToken)
        {
            var badSignatures = await _badSignaturesRepository.FindByIdAsync(request.Id, cancellationToken);
            if (badSignatures is null)
            {
                throw new NotFoundException($"Could not find BadSignatures '{request.Id}'");
            }
            return badSignatures.MapToBadSignaturesDto(_mapper);
        }
    }
}