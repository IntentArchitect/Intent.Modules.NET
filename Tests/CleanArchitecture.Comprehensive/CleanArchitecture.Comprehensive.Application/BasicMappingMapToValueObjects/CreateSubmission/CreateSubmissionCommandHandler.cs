using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Domain.BasicMappingMapToValueObjects;
using CleanArchitecture.Comprehensive.Domain.Entities.BasicMappingMapToValueObjects;
using CleanArchitecture.Comprehensive.Domain.Repositories.BasicMappingMapToValueObjects;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.BasicMappingMapToValueObjects.CreateSubmission
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateSubmissionCommandHandler : IRequestHandler<CreateSubmissionCommand, Guid>
    {
        private readonly ISubmissionRepository _submissionRepository;

        [IntentManaged(Mode.Merge)]
        public CreateSubmissionCommandHandler(ISubmissionRepository submissionRepository)
        {
            _submissionRepository = submissionRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateSubmissionCommand request, CancellationToken cancellationToken)
        {
            var newSubmission = new Submission
            {
                SubmissionType = request.SubmissionType,
                Items = request.Items.Select(x => CreateItem(x)).ToList(),
            };

            _submissionRepository.Add(newSubmission);
            await _submissionRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return newSubmission.Id;
        }

        [IntentManaged(Mode.Fully)]
        public static Item CreateItem(CreateSubmissionItemDto dto)
        {
            return new Item(key: dto.Key, value: dto.Value);
        }
    }
}