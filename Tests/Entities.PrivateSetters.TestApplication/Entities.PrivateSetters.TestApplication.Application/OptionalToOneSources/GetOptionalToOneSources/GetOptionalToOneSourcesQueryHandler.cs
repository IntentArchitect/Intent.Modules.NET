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

namespace Entities.PrivateSetters.TestApplication.Application.OptionalToOneSources.GetOptionalToOneSources
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetOptionalToOneSourcesQueryHandler : IRequestHandler<GetOptionalToOneSourcesQuery, List<OptionalToOneSourceDto>>
    {
        private readonly IOptionalToOneSourceRepository _optionalToOneSourceRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetOptionalToOneSourcesQueryHandler(IOptionalToOneSourceRepository optionalToOneSourceRepository,
            IMapper mapper)
        {
            _optionalToOneSourceRepository = optionalToOneSourceRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<OptionalToOneSourceDto>> Handle(
            GetOptionalToOneSourcesQuery request,
            CancellationToken cancellationToken)
        {
            var optionalToOneSources = await _optionalToOneSourceRepository.FindAllAsync(cancellationToken);
            return optionalToOneSources.MapToOptionalToOneSourceDtoList(_mapper);
        }
    }
}