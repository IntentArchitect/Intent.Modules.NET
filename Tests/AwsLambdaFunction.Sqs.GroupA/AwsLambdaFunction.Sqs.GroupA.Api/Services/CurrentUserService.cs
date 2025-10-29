using System.Threading.Tasks;
using AwsLambdaFunction.Sqs.GroupA.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Identity.CurrentUserService", Version = "1.0")]

namespace AwsLambdaFunction.Sqs.GroupA.Api.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        public CurrentUserService()
        {
        }

        public Task<ICurrentUser?> GetAsync()
        {
            return Task.FromResult<ICurrentUser?>(null);
        }

        public async Task<bool> AuthorizeAsync(string policy)
        {
            return await Task.FromResult(true);
        }

        public async Task<bool> IsInRoleAsync(string role)
        {
            return await Task.FromResult(true);
        }
    }
}