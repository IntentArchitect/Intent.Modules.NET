using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Entities.PrivateSetters.TestApplication.Domain.Common.Exceptions;
using Entities.PrivateSetters.TestApplication.Domain.Repositories.Compositional;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace Entities.PrivateSetters.TestApplication.Application.OneToOneSources.GetOneToOneSourceById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetOneToOneSourceByIdQueryHandler : IRequestHandler<GetOneToOneSourceByIdQuery, OneToOneSourceDto>
    {
        private readonly IOneToOneSourceRepository _oneToOneSourceRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetOneToOneSourceByIdQueryHandler(IOneToOneSourceRepository oneToOneSourceRepository, IMapper mapper)
        {
            _oneToOneSourceRepository = oneToOneSourceRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<OneToOneSourceDto> Handle(
            GetOneToOneSourceByIdQuery request,
            CancellationToken cancellationToken)
        {
            var oneToOneSource = await _oneToOneSourceRepository.FindByIdAsync(request.Id, cancellationToken);
            if (oneToOneSource is null)
            {
                throw new NotFoundException($"Could not find OneToOneSource '{request.Id}'");
            }

            return oneToOneSource.MapToOneToOneSourceDto(_mapper);
        }
    }
}