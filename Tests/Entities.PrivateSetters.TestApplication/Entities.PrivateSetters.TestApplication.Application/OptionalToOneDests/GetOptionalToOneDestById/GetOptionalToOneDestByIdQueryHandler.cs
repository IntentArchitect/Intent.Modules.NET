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

namespace Entities.PrivateSetters.TestApplication.Application.OptionalToOneDests.GetOptionalToOneDestById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetOptionalToOneDestByIdQueryHandler : IRequestHandler<GetOptionalToOneDestByIdQuery, OptionalToOneDestDto>
    {
        private readonly IOptionalToOneDestRepository _optionalToOneDestRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetOptionalToOneDestByIdQueryHandler(IOptionalToOneDestRepository optionalToOneDestRepository,
            IMapper mapper)
        {
            _optionalToOneDestRepository = optionalToOneDestRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<OptionalToOneDestDto> Handle(
            GetOptionalToOneDestByIdQuery request,
            CancellationToken cancellationToken)
        {
            var optionalToOneDest = await _optionalToOneDestRepository.FindByIdAsync(request.Id, cancellationToken);
            if (optionalToOneDest is null)
            {
                throw new NotFoundException($"Could not find OptionalToOneDest '{request.Id}'");
            }

            return optionalToOneDest.MapToOptionalToOneDestDto(_mapper);
        }
    }
}