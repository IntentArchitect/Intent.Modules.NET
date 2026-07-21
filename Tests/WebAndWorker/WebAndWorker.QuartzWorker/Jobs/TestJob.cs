using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Quartz;
using WebAndWorker.Application.OnTest;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.QuartzScheduler.ScheduledJob", Version = "1.0")]

namespace WebAndWorker.QuartzWorker.Jobs
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class TestJob : IJob
    {
        private readonly ISender _mediator;

        [IntentManaged(Mode.Merge)]
        public TestJob(ISender mediator)
        {
            _mediator = mediator;
        }

        [IntentManaged(Mode.Fully)]
        public async Task Execute(IJobExecutionContext context)
        {
            var command = new OnTestCommand();
            await _mediator.Send(command);
        }
    }
}