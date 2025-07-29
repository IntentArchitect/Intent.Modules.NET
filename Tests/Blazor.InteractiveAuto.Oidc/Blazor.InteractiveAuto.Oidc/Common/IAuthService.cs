using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.Authentication.Templates.Server.AuthServiceInterfaceTemplate", Version = "1.0")]

namespace Blazor.InteractiveAuto.Oidc.Common
{
    public interface IAuthService
    {
        Task Login(string email, string password, bool rememberMe, string returnUrl);
    }
}