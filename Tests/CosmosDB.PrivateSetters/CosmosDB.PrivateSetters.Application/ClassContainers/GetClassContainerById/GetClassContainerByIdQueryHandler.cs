using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CosmosDB.PrivateSetters.Domain.Common.Exceptions;
using CosmosDB.PrivateSetters.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CosmosDB.PrivateSetters.Application.ClassContainers.GetClassContainerById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetClassContainerByIdQueryHandler : IRequestHandler<GetClassContainerByIdQuery, ClassContainerDto>
    {
        private readonly IClassContainerRepository _classContainerRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetClassContainerByIdQueryHandler(IClassContainerRepository classContainerRepository, IMapper mapper)
        {
            _classContainerRepository = classContainerRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<ClassContainerDto> Handle(
            GetClassContainerByIdQuery request,
            CancellationToken cancellationToken)
        {
            var classContainer = await _classContainerRepository.FindByIdAsync((request.Id, request.ClassPartitionKey), cancellationToken);
            if (classContainer is null)
            {
                throw new NotFoundException($"Could not find ClassContainer '({request.Id}, {request.ClassPartitionKey})'");
            }

            return classContainer.MapToClassContainerDto(_mapper);
        }
    }
}