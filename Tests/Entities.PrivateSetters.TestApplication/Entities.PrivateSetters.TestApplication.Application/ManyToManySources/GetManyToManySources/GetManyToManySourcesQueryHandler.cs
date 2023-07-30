using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Entities.PrivateSetters.TestApplication.Domain.Repositories.Aggregational;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace Entities.PrivateSetters.TestApplication.Application.ManyToManySources.GetManyToManySources
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetManyToManySourcesQueryHandler : IRequestHandler<GetManyToManySourcesQuery, List<ManyToManySourceDto>>
    {
        private readonly IManyToManySourceRepository _manyToManySourceRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetManyToManySourcesQueryHandler(IManyToManySourceRepository manyToManySourceRepository, IMapper mapper)
        {
            _manyToManySourceRepository = manyToManySourceRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<ManyToManySourceDto>> Handle(
            GetManyToManySourcesQuery request,
            CancellationToken cancellationToken)
        {
            var manyToManySources = await _manyToManySourceRepository.FindAllAsync(cancellationToken);
            return manyToManySources.MapToManyToManySourceDtoList(_mapper);
        }
    }
}