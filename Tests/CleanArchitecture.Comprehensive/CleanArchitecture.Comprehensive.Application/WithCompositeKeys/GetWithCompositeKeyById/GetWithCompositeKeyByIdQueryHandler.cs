using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.Comprehensive.Domain.Common.Exceptions;
using CleanArchitecture.Comprehensive.Domain.Repositories.CompositeKeys;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.WithCompositeKeys.GetWithCompositeKeyById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetWithCompositeKeyByIdQueryHandler : IRequestHandler<GetWithCompositeKeyByIdQuery, WithCompositeKeyDto>
    {
        private readonly IWithCompositeKeyRepository _withCompositeKeyRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetWithCompositeKeyByIdQueryHandler(IWithCompositeKeyRepository withCompositeKeyRepository, IMapper mapper)
        {
            _withCompositeKeyRepository = withCompositeKeyRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<WithCompositeKeyDto> Handle(
            GetWithCompositeKeyByIdQuery request,
            CancellationToken cancellationToken)
        {
            var withCompositeKey = await _withCompositeKeyRepository.FindByIdAsync((request.Key1Id, request.Key2Id), cancellationToken);
            if (withCompositeKey is null)
            {
                throw new NotFoundException($"Could not find WithCompositeKey '({request.Key1Id}, {request.Key2Id})'");
            }

            return withCompositeKey.MapToWithCompositeKeyDto(_mapper);
        }
    }
}