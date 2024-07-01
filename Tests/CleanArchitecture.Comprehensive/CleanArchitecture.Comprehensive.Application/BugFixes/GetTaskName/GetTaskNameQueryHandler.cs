using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.BugFixes.GetTaskName
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetTaskNameQueryHandler : IRequestHandler<GetTaskNameQuery, TaskNameDto>
    {
        [IntentManaged(Mode.Merge)]
        public GetTaskNameQueryHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<TaskNameDto> Handle(GetTaskNameQuery request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (GetTaskNameQueryHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}