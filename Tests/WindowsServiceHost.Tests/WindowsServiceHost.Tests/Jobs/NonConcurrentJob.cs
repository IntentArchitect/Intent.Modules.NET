using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Logging;
using Quartz;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.QuartzScheduler.ScheduledJob", Version = "1.0")]

namespace WindowsServiceHost.Tests.Jobs
{
    [DisallowConcurrentExecution]
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class NonConcurrentJob : IJob
    {
        private readonly ILogger<NonConcurrentJob> _logger;

        [IntentManaged(Mode.Merge)]
        public NonConcurrentJob(ILogger<NonConcurrentJob> logger)
        {
            _logger = logger;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("Non Concurrent");
            await Task.Delay(2000);
        }
    }
}