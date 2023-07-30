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

namespace Entities.PrivateSetters.TestApplication.Application.OptionalToManySources.GetOptionalToManySources
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetOptionalToManySourcesQueryHandler : IRequestHandler<GetOptionalToManySourcesQuery, List<OptionalToManySourceDto>>
    {
        private readonly IOptionalToManySourceRepository _optionalToManySourceRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetOptionalToManySourcesQueryHandler(IOptionalToManySourceRepository optionalToManySourceRepository,
            IMapper mapper)
        {
            _optionalToManySourceRepository = optionalToManySourceRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<OptionalToManySourceDto>> Handle(
            GetOptionalToManySourcesQuery request,
            CancellationToken cancellationToken)
        {
            var optionalToManySources = await _optionalToManySourceRepository.FindAllAsync(cancellationToken);
            return optionalToManySources.MapToOptionalToManySourceDtoList(_mapper);
        }
    }
}