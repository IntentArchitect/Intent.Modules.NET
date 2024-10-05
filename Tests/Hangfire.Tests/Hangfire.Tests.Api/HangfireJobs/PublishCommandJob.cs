using System.Threading.Tasks;
using Hangfire.Tests.Application.Job;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Modules.Hangfire.HangfireJobs", Version = "1.0")]

namespace Hangfire.Tests.Api.HangfireJobs
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class PublishCommandJob
    {
        private readonly ISender _mediator;

        [IntentManaged(Mode.Merge)]
        public PublishCommandJob(ISender mediator)
        {
            _mediator = mediator;
        }

        [AutomaticRetry(Attempts = 10, OnAttemptsExceeded = AttemptsExceededAction.Fail)]
        [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
        public async Task ExecuteAsync()
        {
            var command = new JobCommand();
            await _mediator.Send(command);
        }
    }
}