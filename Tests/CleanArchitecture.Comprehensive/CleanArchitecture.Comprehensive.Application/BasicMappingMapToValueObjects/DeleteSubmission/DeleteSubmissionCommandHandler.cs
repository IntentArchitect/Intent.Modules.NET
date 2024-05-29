using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Domain.Common.Exceptions;
using CleanArchitecture.Comprehensive.Domain.Repositories.BasicMappingMapToValueObjects;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.BasicMappingMapToValueObjects.DeleteSubmission
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteSubmissionCommandHandler : IRequestHandler<DeleteSubmissionCommand>
    {
        private readonly ISubmissionRepository _submissionRepository;

        [IntentManaged(Mode.Merge)]
        public DeleteSubmissionCommandHandler(ISubmissionRepository submissionRepository)
        {
            _submissionRepository = submissionRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DeleteSubmissionCommand request, CancellationToken cancellationToken)
        {
            var existingSubmission = await _submissionRepository.FindByIdAsync(request.Id, cancellationToken);
            if (existingSubmission is null)
            {
                throw new NotFoundException($"Could not find Submission '{request.Id}'");
            }

            _submissionRepository.Remove(existingSubmission);
        }
    }
}