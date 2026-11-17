using Hangfire.Dashboard;
using Intent.RoslynWeaver.Attributes;
using TestMediator.Hangfire.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Hangfire.HangfireDashboardAuthFilter", Version = "1.0")]

namespace TestMediator.Hangfire.Api.Filters
{
    public class HangfireDashboardAuthFilter : IDashboardAuthorizationFilter
    {
        public HangfireDashboardAuthFilter()
        {
        }

        [IntentManaged(Mode.Ignore)]
        public bool Authorize(DashboardContext context)
        {
            var currentUser = context.GetHttpContext().RequestServices.GetRequiredService<ICurrentUserService>();
            return currentUser.GetAsync().GetAwaiter().GetResult()?.Id is not null;
        }
    }
}