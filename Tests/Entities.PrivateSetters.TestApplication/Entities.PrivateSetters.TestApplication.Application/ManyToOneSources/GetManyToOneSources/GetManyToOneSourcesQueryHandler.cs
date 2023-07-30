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

namespace Entities.PrivateSetters.TestApplication.Application.ManyToOneSources.GetManyToOneSources
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetManyToOneSourcesQueryHandler : IRequestHandler<GetManyToOneSourcesQuery, List<ManyToOneSourceDto>>
    {
        private readonly IManyToOneSourceRepository _manyToOneSourceRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetManyToOneSourcesQueryHandler(IManyToOneSourceRepository manyToOneSourceRepository, IMapper mapper)
        {
            _manyToOneSourceRepository = manyToOneSourceRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<ManyToOneSourceDto>> Handle(
            GetManyToOneSourcesQuery request,
            CancellationToken cancellationToken)
        {
            var manyToOneSources = await _manyToOneSourceRepository.FindAllAsync(cancellationToken);
            return manyToOneSources.MapToManyToOneSourceDtoList(_mapper);
        }
    }
}