using System.Security.Claims;
using System.Threading.Tasks;
using CleanArchitecture.TestApplication.Application.Common.Interfaces;
using IdentityModel;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Identity.CurrentUserService", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Api.Services
{
    public class CurrentUserService : ICurrentUserService
    {

        public CurrentUserService()
        {
        }

        public string UserId { get; set; }

        public string UserName { get; set; }

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