using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.Comprehensive.Domain.Repositories.BasicMappingMapToValueObjects;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.BasicMappingMapToValueObjects.GetSubmissions
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetSubmissionsQueryHandler : IRequestHandler<GetSubmissionsQuery, List<SubmissionDto>>
    {
        private readonly ISubmissionRepository _submissionRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetSubmissionsQueryHandler(ISubmissionRepository submissionRepository, IMapper mapper)
        {
            _submissionRepository = submissionRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<SubmissionDto>> Handle(GetSubmissionsQuery request, CancellationToken cancellationToken)
        {
            var submissions = await _submissionRepository.FindAllAsync(cancellationToken);
            return submissions.MapToSubmissionDtoList(_mapper);
        }
    }
}