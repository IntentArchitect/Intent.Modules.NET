using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: DefaultIntentManaged(Mode.Merge, Signature = Mode.Fully, Attributes = Mode.Merge, Comments = Mode.Merge, Targets = Targets.Constructors | Targets.Methods)]
[assembly: IntentTemplate("Intent.Services.Guardrails.Contracts.ServiceContract", Version = "1.0")]

namespace eShop.Ordering.API.Infrastructure.Services;

public class IdentityService(IHttpContextAccessor context) : IIdentityService
{
    public string GetUserIdentity()
        => context.HttpContext?.User.FindFirst("sub")?.Value;

    public string GetUserName()
        => context.HttpContext?.User.Identity?.Name;
}
