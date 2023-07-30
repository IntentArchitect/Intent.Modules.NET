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

namespace Entities.PrivateSetters.TestApplication.Application.ManyToManyDests.GetManyToManyDests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetManyToManyDestsQueryHandler : IRequestHandler<GetManyToManyDestsQuery, List<ManyToManyDestDto>>
    {
        private readonly IManyToManyDestRepository _manyToManyDestRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetManyToManyDestsQueryHandler(IManyToManyDestRepository manyToManyDestRepository, IMapper mapper)
        {
            _manyToManyDestRepository = manyToManyDestRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<ManyToManyDestDto>> Handle(
            GetManyToManyDestsQuery request,
            CancellationToken cancellationToken)
        {
            var manyToManyDests = await _manyToManyDestRepository.FindAllAsync(cancellationToken);
            return manyToManyDests.MapToManyToManyDestDtoList(_mapper);
        }
    }
}