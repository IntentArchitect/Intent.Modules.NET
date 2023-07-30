using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Entities.PrivateSetters.TestApplication.Domain.Common.Exceptions;
using Entities.PrivateSetters.TestApplication.Domain.Repositories.Aggregational;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace Entities.PrivateSetters.TestApplication.Application.OptionalToOneSources.GetOptionalToOneSourceById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetOptionalToOneSourceByIdQueryHandler : IRequestHandler<GetOptionalToOneSourceByIdQuery, OptionalToOneSourceDto>
    {
        private readonly IOptionalToOneSourceRepository _optionalToOneSourceRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetOptionalToOneSourceByIdQueryHandler(IOptionalToOneSourceRepository optionalToOneSourceRepository,
            IMapper mapper)
        {
            _optionalToOneSourceRepository = optionalToOneSourceRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<OptionalToOneSourceDto> Handle(
            GetOptionalToOneSourceByIdQuery request,
            CancellationToken cancellationToken)
        {
            var optionalToOneSource = await _optionalToOneSourceRepository.FindByIdAsync(request.Id, cancellationToken);
            if (optionalToOneSource is null)
            {
                throw new NotFoundException($"Could not find OptionalToOneSource '{request.Id}'");
            }

            return optionalToOneSource.MapToOptionalToOneSourceDto(_mapper);
        }
    }
}