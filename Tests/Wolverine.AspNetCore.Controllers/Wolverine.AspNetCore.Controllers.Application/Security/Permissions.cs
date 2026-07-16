using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Permissions", Version = "1.0")]

namespace Wolverine.AspNetCore.Controllers.Application.Security
{
    public static class Permissions
    {
        public const string RoleAdministrator = "Administrator";

        public static IEnumerable<string> All()
        {
            yield return RoleAdministrator;
        }

        public static IEnumerable<string> Roles()
        {
            yield return RoleAdministrator;
        }

        public static IEnumerable<string> Policies()
        {
            yield break;
        }
    }
}