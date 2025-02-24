using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Permissions", Version = "1.0")]

namespace AspNetCore.Controllers.Secured.Application.Security
{
    public static class Permissions
    {
        public const string Role1 = "Role1";
        public const string Role2 = "Role2";
        public const string Policy1 = "Policy1";
        public const string Policy2 = "Policy2";
        public const string RoleAdmin = "Admin";
        public const string RoleUser = "User";

        public static IEnumerable<string> All()
        {
            yield return Role1;
            yield return Role2;
            yield return Policy1;
            yield return Policy2;
            yield return RoleAdmin;
            yield return RoleUser;
        }

        public static IEnumerable<string> Roles()
        {
            yield return Role1;
            yield return Role2;
            yield return RoleAdmin;
            yield return RoleUser;
        }

        public static IEnumerable<string> Policies()
        {
            yield return Policy1;
            yield return Policy2;
        }
    }
}