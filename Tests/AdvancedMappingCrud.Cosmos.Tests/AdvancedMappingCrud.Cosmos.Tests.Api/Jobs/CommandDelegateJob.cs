using System.Threading.Tasks;
using AdvancedMappingCrud.Cosmos.Tests.Application.Jobs.MyTimedJob;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Quartz;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.QuartzScheduler.ScheduledJob", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Api.Jobs
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CommandDelegateJob : IJob
    {
        private readonly ISender _mediator;

        [IntentManaged(Mode.Merge)]
        public CommandDelegateJob(ISender mediator)
        {
            _mediator = mediator;
        }

        [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
        public async Task Execute(IJobExecutionContext context)
        {
            var command = new MyTimedJobCommand();
            await _mediator.Send(command);
        }
    }
}