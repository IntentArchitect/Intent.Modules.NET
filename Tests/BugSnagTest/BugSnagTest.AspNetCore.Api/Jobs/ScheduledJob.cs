using System.Threading.Tasks;
using BugSnagTest.AspNetCore.Application.Scheduled;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Quartz;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.QuartzScheduler.ScheduledJob", Version = "1.0")]

namespace BugSnagTest.AspNetCore.Api.Jobs
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ScheduledJob : IJob
    {
        private readonly ISender _mediator;

        [IntentManaged(Mode.Merge)]
        public ScheduledJob(ISender mediator)
        {
            _mediator = mediator;
        }

        [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
        public async Task Execute(IJobExecutionContext context)
        {
            var command = new ScheduledCommand();
            await _mediator.Send(command);
        }
    }
}