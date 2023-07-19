using System;
using System.Threading.Tasks;
using EntityFrameworkCore.SqlServer.TestApplication.Application.Common.Interfaces;

namespace EntityFrameworkCore.SqlServer.TestApplication.Infrastructure.Services;

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
}