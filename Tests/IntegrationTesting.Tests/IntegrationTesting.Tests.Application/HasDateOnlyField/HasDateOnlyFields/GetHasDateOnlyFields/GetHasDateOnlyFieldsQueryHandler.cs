using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using IntegrationTesting.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace IntegrationTesting.Tests.Application.HasDateOnlyField.HasDateOnlyFields.GetHasDateOnlyFields
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetHasDateOnlyFieldsQueryHandler : IRequestHandler<GetHasDateOnlyFieldsQuery, List<HasDateOnlyFieldDto>>
    {
        private readonly IHasDateOnlyFieldRepository _hasDateOnlyFieldRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetHasDateOnlyFieldsQueryHandler(IHasDateOnlyFieldRepository hasDateOnlyFieldRepository, IMapper mapper)
        {
            _hasDateOnlyFieldRepository = hasDateOnlyFieldRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<HasDateOnlyFieldDto>> Handle(
            GetHasDateOnlyFieldsQuery request,
            CancellationToken cancellationToken)
        {
            var hasDateOnlyFields = await _hasDateOnlyFieldRepository.FindAllAsync(cancellationToken);
            return hasDateOnlyFields.MapToHasDateOnlyFieldDtoList(_mapper);
        }
    }
}