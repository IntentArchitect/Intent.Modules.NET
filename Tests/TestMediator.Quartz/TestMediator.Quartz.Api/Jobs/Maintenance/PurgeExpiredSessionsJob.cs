using Intent.RoslynWeaver.Attributes;
using Mediator;
using Quartz;
using TestMediator.Quartz.Application.Maintenance.PurgeExpiredSessions;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.QuartzScheduler.ScheduledJob", Version = "1.0")]

namespace TestMediator.Quartz.Api.Jobs.Maintenance
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class PurgeExpiredSessionsJob : IJob
    {
        private readonly ISender _mediator;

        [IntentManaged(Mode.Merge)]
        public PurgeExpiredSessionsJob(ISender mediator)
        {
            _mediator = mediator;
        }

        [IntentManaged(Mode.Fully)]
        public async Task Execute(IJobExecutionContext context)
        {
            var command = new PurgeExpiredSessionsCommand();
            await _mediator.Send(command);
        }
    }
}