using System;
using System.Security.Claims;
using System.Threading.Tasks;
using EntityFrameworkCore.CosmosDb.TestApplication.Application.Common.Interfaces;

namespace EntityFrameworkCore.CosmosDb.TestApplication.Infrastructure.Services;

public class DummyCurrentUserService : ICurrentUserService
{
    public string? UserId { get; set; } = "user@test.com";
    public string? UserName { get; set; } = "Test User";
    
    public Task<bool> IsInRoleAsync(string role)
    {
        return Task.FromResult(true);
    }

    public Task<bool> AuthorizeAsync(string policy)
    {
        return Task.FromResult(true);
    }

    public Task<ICurrentUser?> GetAsync()
    {
        ICurrentUser? user = new CurrentUser("user@test.com", "Test User", null);
        return Task.FromResult<ICurrentUser?>(user);
    }
}

public record CurrentUser(string? Id, string? Name, ClaimsPrincipal Principal) : ICurrentUser;