using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Repositories.MappableStoredProcs;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Application.MappableStoredProcs.GetEntityById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetEntityByIdHandler : IRequestHandler<GetEntityById, EntityDto>
    {
        private readonly IMappableStoredProcRepository _mappableStoredProcRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetEntityByIdHandler(IMappableStoredProcRepository mappableStoredProcRepository, IMapper mapper)
        {
            _mappableStoredProcRepository = mappableStoredProcRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<EntityDto> Handle(GetEntityById request, CancellationToken cancellationToken)
        {
            var result = await _mappableStoredProcRepository.GetEntityById(request.Id, cancellationToken);
            return result.MapToEntityDto(_mapper);
        }
    }
}