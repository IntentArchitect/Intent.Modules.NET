using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Permissions", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF7.Application.Security
{
    public static class Permissions
    {
        public const string RoleAdmin = "admin";
        public const string PolicyUser = "user";

        public static IEnumerable<string> All()
        {
            yield return RoleAdmin;
            yield return PolicyUser;
        }

        public static IEnumerable<string> Roles()
        {
            yield return RoleAdmin;
        }

        public static IEnumerable<string> Policies()
        {
            yield return PolicyUser;
        }
    }
}