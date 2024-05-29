using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Domain.BasicMappingMapToValueObjects;
using CleanArchitecture.Comprehensive.Domain.Common.Exceptions;
using CleanArchitecture.Comprehensive.Domain.Repositories.BasicMappingMapToValueObjects;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.BasicMappingMapToValueObjects.UpdateSubmission
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateSubmissionCommandHandler : IRequestHandler<UpdateSubmissionCommand>
    {
        private readonly ISubmissionRepository _submissionRepository;

        [IntentManaged(Mode.Merge)]
        public UpdateSubmissionCommandHandler(ISubmissionRepository submissionRepository)
        {
            _submissionRepository = submissionRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateSubmissionCommand request, CancellationToken cancellationToken)
        {
            var existingSubmission = await _submissionRepository.FindByIdAsync(request.Id, cancellationToken);
            if (existingSubmission is null)
            {
                throw new NotFoundException($"Could not find Submission '{request.Id}'");
            }

            existingSubmission.SubmissionType = request.SubmissionType;
            existingSubmission.Items = request.Items.Select(x => CreateItem(x)).ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static Item CreateItem(UpdateSubmissionItemDto dto)
        {
            return new Item(key: dto.Key, value: dto.Value);
        }
    }
}