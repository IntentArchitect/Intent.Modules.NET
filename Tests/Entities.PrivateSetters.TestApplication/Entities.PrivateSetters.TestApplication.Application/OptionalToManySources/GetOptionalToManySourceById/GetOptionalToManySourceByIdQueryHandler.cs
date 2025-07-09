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

namespace Entities.PrivateSetters.TestApplication.Application.OptionalToManySources.GetOptionalToManySourceById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetOptionalToManySourceByIdQueryHandler : IRequestHandler<GetOptionalToManySourceByIdQuery, OptionalToManySourceDto>
    {
        private readonly IOptionalToManySourceRepository _optionalToManySourceRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetOptionalToManySourceByIdQueryHandler(IOptionalToManySourceRepository optionalToManySourceRepository,
            IMapper mapper)
        {
            _optionalToManySourceRepository = optionalToManySourceRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<OptionalToManySourceDto> Handle(
            GetOptionalToManySourceByIdQuery request,
            CancellationToken cancellationToken)
        {
            var optionalToManySource = await _optionalToManySourceRepository.FindByIdAsync(request.Id, cancellationToken);
            if (optionalToManySource is null)
            {
                throw new NotFoundException($"Could not find OptionalToManySource '{request.Id}'");
            }

            return optionalToManySource.MapToOptionalToManySourceDto(_mapper);
        }
    }
}