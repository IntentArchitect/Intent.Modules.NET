using System;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Logging;
using Quartz;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.QuartzScheduler.ScheduledJob", Version = "1.0")]

namespace WindowsServiceHost.Tests.Jobs
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class RunEvery15Seconds : IJob
    {
        private readonly ILogger<RunEvery15Seconds> _logger;

        [IntentManaged(Mode.Merge)]
        public RunEvery15Seconds(ILogger<RunEvery15Seconds> logger)
        {
            _logger = logger;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("15 seconds");
        }
    }
}