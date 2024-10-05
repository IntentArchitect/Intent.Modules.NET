using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using IntegrationTesting.Tests.Domain.Common.Exceptions;
using IntegrationTesting.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace IntegrationTesting.Tests.Application.HasDateOnlyField.HasDateOnlyFields.GetHasDateOnlyFieldById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetHasDateOnlyFieldByIdQueryHandler : IRequestHandler<GetHasDateOnlyFieldByIdQuery, HasDateOnlyFieldDto>
    {
        private readonly IHasDateOnlyFieldRepository _hasDateOnlyFieldRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetHasDateOnlyFieldByIdQueryHandler(IHasDateOnlyFieldRepository hasDateOnlyFieldRepository, IMapper mapper)
        {
            _hasDateOnlyFieldRepository = hasDateOnlyFieldRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<HasDateOnlyFieldDto> Handle(
            GetHasDateOnlyFieldByIdQuery request,
            CancellationToken cancellationToken)
        {
            var hasDateOnlyField = await _hasDateOnlyFieldRepository.FindByIdAsync(request.Id, cancellationToken);
            if (hasDateOnlyField is null)
            {
                throw new NotFoundException($"Could not find HasDateOnlyField '{request.Id}'");
            }
            return hasDateOnlyField.MapToHasDateOnlyFieldDto(_mapper);
        }
    }
}