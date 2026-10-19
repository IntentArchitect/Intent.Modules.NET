using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Permissions", Version = "1.0")]

namespace TestMediator.Publisher.Application.Security
{
    public static class Permissions
    {
        public const string RoleAdmin = "Admin";

        public static IEnumerable<string> All()
        {
            yield return RoleAdmin;
        }

        public static IEnumerable<string> Roles()
        {
            yield return RoleAdmin;
        }

        public static IEnumerable<string> Policies()
        {
            yield break;
        }
    }
}