using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace FluentValidationTest.Application.ValidationScenarios.StressSuite.UpdateProjectTags
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateProjectTagsCommandHandler : IRequestHandler<UpdateProjectTagsCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateProjectTagsCommandHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task Handle(UpdateProjectTagsCommand request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (UpdateProjectTagsCommandHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}