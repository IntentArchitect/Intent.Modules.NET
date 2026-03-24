using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Identity.CurrentUserServiceInterface", Version = "1.0")]

namespace JsonPatchRfc7396.Scalar.Application.Common.Interfaces
{
    public interface ICurrentUserService
    {
        Task<ICurrentUser?> GetAsync();
        Task<bool> IsInRoleAsync(string role);
        Task<bool> AuthorizeAsync(string policy);
    }
}