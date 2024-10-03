using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Application.Hangfire.Hangfire;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Modules.Hangfire.HangfireJobs", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Api.HangfireJobs
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CommandJob
    {
        private readonly ISender _mediator;

        [IntentManaged(Mode.Merge)]
        public CommandJob(ISender mediator)
        {
            _mediator = mediator;
        }

        [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
        public async Task ExecuteAsync()
        {
            var command = new HangfireCommand();
            await _mediator.Send(command);
        }
    }
}