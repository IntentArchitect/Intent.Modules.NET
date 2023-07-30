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

namespace Entities.PrivateSetters.TestApplication.Application.OptionalToManyDests.GetOptionalToManyDests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetOptionalToManyDestsQueryHandler : IRequestHandler<GetOptionalToManyDestsQuery, List<OptionalToManyDestDto>>
    {
        private readonly IOptionalToManyDestRepository _optionalToManyDestRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetOptionalToManyDestsQueryHandler(IOptionalToManyDestRepository optionalToManyDestRepository,
            IMapper mapper)
        {
            _optionalToManyDestRepository = optionalToManyDestRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<OptionalToManyDestDto>> Handle(
            GetOptionalToManyDestsQuery request,
            CancellationToken cancellationToken)
        {
            var optionalToManyDests = await _optionalToManyDestRepository.FindAllAsync(cancellationToken);
            return optionalToManyDests.MapToOptionalToManyDestDtoList(_mapper);
        }
    }
}