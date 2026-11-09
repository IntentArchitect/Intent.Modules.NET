using Hangfire;
using Intent.RoslynWeaver.Attributes;
using Mediator;
using TestMediator.Hangfire.Application.Maintenance.PurgeExpiredSessions;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Hangfire.HangfireJobs", Version = "1.0")]

namespace TestMediator.Hangfire.Api.HangfireJobs.Maintenance
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class PurgeExpiredSessionsJob
    {
        private readonly ISender _mediator;

        [IntentManaged(Mode.Merge)]
        public PurgeExpiredSessionsJob(ISender mediator)
        {
            _mediator = mediator;
        }

        [AutomaticRetry(Attempts = 10, OnAttemptsExceeded = AttemptsExceededAction.Fail)]
        [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
        public async Task ExecuteAsync()
        {
            var command = new PurgeExpiredSessionsCommand();
            await _mediator.Send(command);
        }
    }
}