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

namespace Entities.PrivateSetters.TestApplication.Application.OptionalToOneDests.GetOptionalToOneDests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetOptionalToOneDestsQueryHandler : IRequestHandler<GetOptionalToOneDestsQuery, List<OptionalToOneDestDto>>
    {
        private readonly IOptionalToOneDestRepository _optionalToOneDestRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetOptionalToOneDestsQueryHandler(IOptionalToOneDestRepository optionalToOneDestRepository, IMapper mapper)
        {
            _optionalToOneDestRepository = optionalToOneDestRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<OptionalToOneDestDto>> Handle(
            GetOptionalToOneDestsQuery request,
            CancellationToken cancellationToken)
        {
            var optionalToOneDests = await _optionalToOneDestRepository.FindAllAsync(cancellationToken);
            return optionalToOneDests.MapToOptionalToOneDestDtoList(_mapper);
        }
    }
}