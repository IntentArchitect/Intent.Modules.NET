using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Entities.PrivateSetters.TestApplication.Domain.Repositories.Compositional;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace Entities.PrivateSetters.TestApplication.Application.OneToOneSources.GetOneToOneSources
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetOneToOneSourcesQueryHandler : IRequestHandler<GetOneToOneSourcesQuery, List<OneToOneSourceDto>>
    {
        private readonly IOneToOneSourceRepository _oneToOneSourceRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetOneToOneSourcesQueryHandler(IOneToOneSourceRepository oneToOneSourceRepository, IMapper mapper)
        {
            _oneToOneSourceRepository = oneToOneSourceRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<OneToOneSourceDto>> Handle(
            GetOneToOneSourcesQuery request,
            CancellationToken cancellationToken)
        {
            var oneToOneSources = await _oneToOneSourceRepository.FindAllAsync(cancellationToken);
            return oneToOneSources.MapToOneToOneSourceDtoList(_mapper);
        }
    }
}