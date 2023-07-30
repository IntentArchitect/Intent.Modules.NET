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

namespace Entities.PrivateSetters.TestApplication.Application.OneToOptionalSources.GetOneToOptionalSourceById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetOneToOptionalSourceByIdQueryHandler : IRequestHandler<GetOneToOptionalSourceByIdQuery, OneToOptionalSourceDto>
    {
        private readonly IOneToOptionalSourceRepository _oneToOptionalSourceRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetOneToOptionalSourceByIdQueryHandler(IOneToOptionalSourceRepository oneToOptionalSourceRepository,
            IMapper mapper)
        {
            _oneToOptionalSourceRepository = oneToOptionalSourceRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<OneToOptionalSourceDto> Handle(
            GetOneToOptionalSourceByIdQuery request,
            CancellationToken cancellationToken)
        {
            var oneToOptionalSource = await _oneToOptionalSourceRepository.FindByIdAsync(request.Id, cancellationToken);
            if (oneToOptionalSource is null)
            {
                throw new NotFoundException($"Could not find OneToOptionalSource '{request.Id}'");
            }

            return oneToOptionalSource.MapToOneToOptionalSourceDto(_mapper);
        }
    }
}