using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.Comprehensive.Domain.Repositories.CompositeKeys;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.WithCompositeKeys.GetWithCompositeKeys
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetWithCompositeKeysQueryHandler : IRequestHandler<GetWithCompositeKeysQuery, List<WithCompositeKeyDto>>
    {
        private readonly IWithCompositeKeyRepository _withCompositeKeyRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetWithCompositeKeysQueryHandler(IWithCompositeKeyRepository withCompositeKeyRepository, IMapper mapper)
        {
            _withCompositeKeyRepository = withCompositeKeyRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<WithCompositeKeyDto>> Handle(
            GetWithCompositeKeysQuery request,
            CancellationToken cancellationToken)
        {
            var withCompositeKeys = await _withCompositeKeyRepository.FindAllAsync(cancellationToken);
            return withCompositeKeys.MapToWithCompositeKeyDtoList(_mapper);
        }
    }
}