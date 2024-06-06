using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.Comprehensive.Domain.Common.Exceptions;
using CleanArchitecture.Comprehensive.Domain.Repositories.BasicMappingMapToValueObjects;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.BasicMappingMapToValueObjects.GetSubmissionById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetSubmissionByIdQueryHandler : IRequestHandler<GetSubmissionByIdQuery, SubmissionDto>
    {
        private readonly ISubmissionRepository _submissionRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetSubmissionByIdQueryHandler(ISubmissionRepository submissionRepository, IMapper mapper)
        {
            _submissionRepository = submissionRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<SubmissionDto> Handle(GetSubmissionByIdQuery request, CancellationToken cancellationToken)
        {
            var submission = await _submissionRepository.FindByIdAsync(request.Id, cancellationToken);
            if (submission is null)
            {
                throw new NotFoundException($"Could not find Submission '{request.Id}'");
            }

            return submission.MapToSubmissionDto(_mapper);
        }
    }
}