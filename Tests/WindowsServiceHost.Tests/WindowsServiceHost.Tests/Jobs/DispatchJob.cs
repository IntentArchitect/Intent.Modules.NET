using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Quartz;
using WindowsServiceHost.Tests.MyScheduled;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.QuartzScheduler.ScheduledJob", Version = "1.0")]

namespace WindowsServiceHost.Tests.Jobs
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DispatchJob : IJob
    {
        private readonly ISender _mediator;

        [IntentManaged(Mode.Merge)]
        public DispatchJob(ISender mediator)
        {
            _mediator = mediator;
        }

        [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
        public async Task Execute(IJobExecutionContext context)
        {
            var command = new MyScheduledCommand();
            await _mediator.Send(command);
        }
    }
}