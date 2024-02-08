using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Redis.Om.Repositories.Domain.Common.Exceptions;
using Redis.Om.Repositories.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace Redis.Om.Repositories.Application.DerivedTypes.GetDerivedTypeById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetDerivedTypeByIdQueryHandler : IRequestHandler<GetDerivedTypeByIdQuery, DerivedTypeDto>
    {
        private readonly IDerivedTypeRepository _derivedTypeRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetDerivedTypeByIdQueryHandler(IDerivedTypeRepository derivedTypeRepository, IMapper mapper)
        {
            _derivedTypeRepository = derivedTypeRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<DerivedTypeDto> Handle(GetDerivedTypeByIdQuery request, CancellationToken cancellationToken)
        {
            var derivedType = await _derivedTypeRepository.FindByIdAsync(request.Id, cancellationToken);
            if (derivedType is null)
            {
                throw new NotFoundException($"Could not find DerivedType '{request.Id}'");
            }
            return derivedType.MapToDerivedTypeDto(_mapper);
        }
    }
}