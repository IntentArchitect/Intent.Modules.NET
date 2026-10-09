namespace CqrsSpike.Subscriber.Application.Common.Interfaces
{
    public interface ICurrentUserService
    {
        Task<ICurrentUser?> GetAsync();
        Task<bool> IsInRoleAsync(string role);
        Task<bool> AuthorizeAsync(string policy);
    }
}