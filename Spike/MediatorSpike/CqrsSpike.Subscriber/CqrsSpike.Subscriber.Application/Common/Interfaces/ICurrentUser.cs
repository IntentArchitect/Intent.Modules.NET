using System.Security.Claims;

namespace CqrsSpike.Subscriber.Application.Common.Interfaces
{
    public interface ICurrentUser
    {
        string? Id { get; }
        string? Name { get; }
        ClaimsPrincipal Principal { get; }
    }
}