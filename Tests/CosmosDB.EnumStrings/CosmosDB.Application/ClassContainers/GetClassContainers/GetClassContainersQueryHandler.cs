using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CosmosDB.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CosmosDB.Application.ClassContainers.GetClassContainers
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetClassContainersQueryHandler : IRequestHandler<GetClassContainersQuery, List<ClassContainerDto>>
    {
        private readonly IClassContainerRepository _classContainerRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetClassContainersQueryHandler(IClassContainerRepository classContainerRepository, IMapper mapper)
        {
            _classContainerRepository = classContainerRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<ClassContainerDto>> Handle(
            GetClassContainersQuery request,
            CancellationToken cancellationToken)
        {
            var classContainers = await _classContainerRepository.FindAllAsync(cancellationToken);
            return classContainers.MapToClassContainerDtoList(_mapper);
        }
    }
}