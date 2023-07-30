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

namespace Entities.PrivateSetters.TestApplication.Application.ManyToOneDests.GetManyToOneDests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetManyToOneDestsQueryHandler : IRequestHandler<GetManyToOneDestsQuery, List<ManyToOneDestDto>>
    {
        private readonly IManyToOneDestRepository _manyToOneDestRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetManyToOneDestsQueryHandler(IManyToOneDestRepository manyToOneDestRepository, IMapper mapper)
        {
            _manyToOneDestRepository = manyToOneDestRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<ManyToOneDestDto>> Handle(
            GetManyToOneDestsQuery request,
            CancellationToken cancellationToken)
        {
            var manyToOneDests = await _manyToOneDestRepository.FindAllAsync(cancellationToken);
            return manyToOneDests.MapToManyToOneDestDtoList(_mapper);
        }
    }
}