using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Quartz;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.QuartzScheduler.ScheduledJob", Version = "1.0")]

namespace WebAndWorker.QuartzWorker.Jobs
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class HandWrittenJob : IJob
    {
        [IntentManaged(Mode.Merge)]
        public HandWrittenJob()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async ValueTask Execute(IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            // Hand-written body: must survive the Quartz 4 signature change (ValueTask + CancellationToken)
            await Task.Delay(TimeSpan.FromMilliseconds(10));
            Console.WriteLine($"HandWrittenJob fired at {context.FireTimeUtc:O}");
        }
    }
}
