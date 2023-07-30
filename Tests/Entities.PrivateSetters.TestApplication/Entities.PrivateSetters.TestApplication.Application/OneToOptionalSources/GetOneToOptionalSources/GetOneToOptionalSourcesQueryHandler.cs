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

namespace Entities.PrivateSetters.TestApplication.Application.OneToOptionalSources.GetOneToOptionalSources
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetOneToOptionalSourcesQueryHandler : IRequestHandler<GetOneToOptionalSourcesQuery, List<OneToOptionalSourceDto>>
    {
        private readonly IOneToOptionalSourceRepository _oneToOptionalSourceRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetOneToOptionalSourcesQueryHandler(IOneToOptionalSourceRepository oneToOptionalSourceRepository,
            IMapper mapper)
        {
            _oneToOptionalSourceRepository = oneToOptionalSourceRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<OneToOptionalSourceDto>> Handle(
            GetOneToOptionalSourcesQuery request,
            CancellationToken cancellationToken)
        {
            var oneToOptionalSources = await _oneToOptionalSourceRepository.FindAllAsync(cancellationToken);
            return oneToOptionalSources.MapToOneToOptionalSourceDtoList(_mapper);
        }
    }
}